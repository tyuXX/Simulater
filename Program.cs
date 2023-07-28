using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Simulater
{
    internal static class Program
    {
        static readonly Random rng = new Random();
        static List<Entity> Entities = new List<Entity>() { };
        static Ticker MainTicker;
        static BigInteger TotalDeaths = 0;
        static BigInteger AverageDeaths = 0;
        static void Main()
        {
            GenerateEntities(int.Parse(GetText("Entities")));
            MainTicker = new Ticker(Tick,int.Parse(GetText("Tick Interval")));
            MainTicker.Start();
            while (true) ;
        }
        static bool IsDead(Entity entity)
        {
            return entity.isDead;
        }
        static bool Tick()
        {
            Stopwatch sw = Stopwatch.StartNew();
            int Killers = 0;
            int Saviors = 0;
            int Cowards = 0;
            for (int i = 0; i < Entities.Count/3; i++)
            {
                Console.Title = $"Processing:{i}/{Entities.Count/3}";
                Entity ______ = new Entity();
                int __ = rng.Next( Entities.Count );
                while (Entities.ElementAt(__).isDead)
                {
                    __ = rng.Next( Entities.Count );
                }
                int ____ = rng.Next( Entities.Count );
                while (Entities.ElementAt( __ ).Equals( Entities.ElementAt( ____ ) ) || Entities.ElementAt(____).isDead)
                {
                    ____ = rng.Next( Entities.Count );
                }
                if(Entities.ElementAt( __ ).entityType == EntityType.Savior || Entities.ElementAt( ____ ).entityType == EntityType.Savior && Entities.ElementAt( __ ).entityType == EntityType.Killer || Entities.ElementAt( ____ ).entityType == EntityType.Killer)
                {
                    switch (rng.Next(2))
                    {
                        case 0:
                            {
                                ______ = Entities.ElementAt( __ );
                                break;
                            }
                        case 1:
                            {
                                ______ = Entities.ElementAt( ____ );
                                break;
                            }
                        default:
                            { 
                                break;
                            }
                    }
                }
                if(Entities.ElementAt( __ ).entityType == EntityType.Coward || Entities.ElementAt( ____ ).entityType == EntityType.Coward && Entities.ElementAt( __ ).entityType == EntityType.Killer || Entities.ElementAt( ____ ).entityType == EntityType.Killer)
                {
                    switch (rng.Next(2))
                    {
                        case 0:
                            {
                                if(Entities.ElementAt( __ ).entityType == EntityType.Coward)
                                {
                                    ______ = Entities.ElementAt( __ );
                                }
                                if (Entities.ElementAt( ____ ).entityType == EntityType.Coward)
                                {
                                    ______ = Entities.ElementAt( ____ );
                                }
                                break;
                            }
                        case 1:
                            {
                                break;
                            }
                        default:
                            { 
                                break;
                            }
                    }
                }
                if (Entities.Contains( ______ ))
                {
                    Entities.Remove( ______ );
                    ______.isDead = true;
                    Entities.Add( ______ );
                }
            }
            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i] = ETick(Entities.ElementAt( i ));
            }
            int DeathOnTick = Entities.RemoveAll( new Predicate<Entity>( IsDead ) );
            int _ = Entities.Count;
            for (int i = 0; i < _; i++)
            {
                Console.Title = $"Processing:{i}/{_}";
                if (rng.Next(4) == 0) { Entities.Add( new Entity( Entities.ElementAt( i ).entityType, Entities.ElementAt( i ).power + rng.Next( -2, 2 ), rng.Next( 1, 3 ) ) ); }
            }
            for (int i = 0; i < Entities.Count; i++)
            {
                Console.Title = $"Processing:{i}/{_}";
                if (Entities.ElementAt( i ).entityType == EntityType.Savior) { Saviors++; }
                else if (Entities.ElementAt( i ).entityType == EntityType.Killer) { Killers++; }
                else if (Entities.ElementAt( i ).entityType == EntityType.Coward) { Cowards++; }
            }
            TotalDeaths += DeathOnTick;
            AverageDeaths = TotalDeaths / (MainTicker.CallbackCount + 1);
            sw.Stop();
            Console.Clear();
            Console.WriteLine($"Ticks:{MainTicker.CallbackCount}\n\nLiving:\nCowards:{Cowards}\nSaviors:{Saviors}\nKillers:{Killers}\nTotal:{Entities.Count}\n\nDead:\nThis tick:{DeathOnTick}\nTotal:{TotalDeaths}\nAverage:{AverageDeaths}\n\nTick took:{sw.ElapsedMilliseconds}ms");
            return true;
        }
        static Entity ETick(Entity entity)
        {
            Entity _ = new Entity();
            entity.CopyTo(out _ );
            Random rng = new Random();
            entity.hunger--;
            if (entity.hunger <= 0) { entity.isDead = true; }
            entity.hunger += rng.Next(-2 , 3 );
            return _;
        }
        static void GenerateEntities(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Entities.Add(new Entity( I2ET( rng.Next( 3 ) ),rng.Next( 3 ) ,rng.Next(1,3)));
            }
        }
        static string GetText(string question)
        {
            Console.Clear();
            Console.Write(question + ':');
            return Console.ReadLine();
        }
        static EntityType I2ET(int i)
        {
            switch (i)
            {
                case 0:
                    {
                        return EntityType.Savior;
                    }
                case 1:
                    {
                        return EntityType.Coward;
                    }
                case 2:
                    {
                        return EntityType.Killer;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException("i",i,"Not in bounds");
                    }
            }
        }
    }
    internal struct Ticker
    {
        private readonly System.Timers.Timer ticker;
        private readonly Func<bool> CallbackFunc;
        public BigInteger CallbackCount;
        public Ticker(Func<bool> callbackfunc,double interval)
        {
            CallbackCount = 0;
            CallbackFunc = callbackfunc;
            ticker = new System.Timers.Timer( interval )
            {
                AutoReset = true
            };
            ticker.Elapsed += OnTick;
        }
        private void OnTick(object sender,ElapsedEventArgs e)
        {
            CallbackCount++;
            CallbackFunc();
        }
        public void Start()
        {
            ticker.Start();
        }
        public void Stop()
        {
            ticker.Stop();
        }
    }
    internal struct Entity
    {
        private Guid UUID;
        public EntityType entityType;
        public int power;
        public int hunger;
        public bool isDead;
        public Entity(EntityType entityType,int power,int hunger)
        {
            UUID = Guid.NewGuid();
            this.entityType = entityType;
            this.power = power;
            this.hunger = hunger;
            isDead = false;
        }
        public void CopyTo(out Entity entity)
        {
            Entity _ = new Entity(entityType,power,hunger);
            _.UUID = UUID;
            entity = _;
        }
        public bool IsSame(Entity entity)
        {
            return Equals(entity);
        }
    }
    internal enum EntityType
    {
        Savior,
        Coward,
        Killer
    }
}
