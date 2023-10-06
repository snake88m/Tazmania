using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;
using Tazmania.Interfaces.Repositories;

namespace Tazmania.EntityFramework.Contexts
{
    public class TazmaniaDbContext : DbContext
    {
        public virtual DbSet<IO> IOs { get; set; } = null!;

        public virtual DbSet<IOGroup> IOGroups { get; set; } = null!;

        public virtual DbSet<Heating> Heatings { get; set; } = null!;

        public virtual DbSet<HeatingSetting> HeatingSettings { get; set; } = null!;

        public virtual DbSet<HeatingTime> HeatingTimes { get; set; } = null!;

        public virtual DbSet<Irrigation> Irrigations { get; set; } = null!;

        public virtual DbSet<IrrigationSetting> IrrigationSettings { get; set; } = null!;

        public virtual DbSet<Scheduler> Schedulers { get; set; } = null!;

        public virtual DbSet<SecuritySetting> SecuritySettings { get; set; } = null!;

        public virtual DbSet<NotifySetting> NotifySettings { get; set; } = null!;

        public virtual DbSet<Request> Requests { get; set; } = null!;

        public virtual DbSet<User> Users { get; set; } = null!;

        public TazmaniaDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // disattivo l'eliminazione a cascata
            modelBuilder.Entity<Heating>()
                .HasOne(r => r.OutputValve)
                .WithMany()
                .HasForeignKey(r => r.OutputValveId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Heating>()
                .HasOne(r => r.InputSensor)
                .WithMany()
                .HasForeignKey(r => r.InputSensorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NotifySetting>()
                .HasOne(r => r.DialerSOS)
                .WithMany()
                .HasForeignKey(r => r.DialerSOSId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NotifySetting>()
                .HasOne(r => r.DialerAutomatic)
                .WithMany()
                .HasForeignKey(r => r.DialerAutomaticId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IOGroup>().HasData(
                new IOGroup() { Id = 1, Name = "Cucina", Order = 1 },
                new IOGroup() { Id = 2, Name = "Salotto", Order = 2 },
                new IOGroup() { Id = 3, Name = "Disimpegno", Order = 3 },
                new IOGroup() { Id = 4, Name = "Studio", Order = 4 },
                new IOGroup() { Id = 5, Name = "Cameretta", Order = 5 },
                new IOGroup() { Id = 6, Name = "Bagno", Order = 6 },
                new IOGroup() { Id = 7, Name = "Camera da letto", Order = 7 },
                new IOGroup() { Id = 8, Name = "Bagno camera da letto", Order = 8 },
                new IOGroup() { Id = 9, Name = "Giardino", Order = 9 }
           );


            ///
            /// QUI VA INSERITA LA MAPPATURA RAM DEL MODULO DFCP 4 ETH
            /// LA MAPPATURA VARIA DA CASA A CASA. HO LASCIATO ALCUNI ESEMPI COME SPUNTO
            ///
           modelBuilder.Entity<IO>().HasData(
                // Queste 4 righe mappano 4 punti luci
                new IO() { Id = 1, DFCPAddress = 1425, ModulePin = 1, Major = IOMajor.Light, IOGroupId = 7, Description = "Comodino SX" },
                new IO() { Id = 2, DFCPAddress = 1425, ModulePin = 2, Major = IOMajor.Light, IOGroupId = 7, Description = "LED soffitto" },
                new IO() { Id = 3, DFCPAddress = 1425, ModulePin = 3, Major = IOMajor.Light, IOGroupId = 7, Description = "Cabina armadio" },
                new IO() { Id = 4, DFCPAddress = 1425, ModulePin = 0, Major = IOMajor.Light, IOGroupId = 7, Description = "Comodino DX" },

                // questa riga mappa un punto luce virtuale
                new IO() { Id = 26, DFCPAddress = 1449, DFCPVirtual = 801, ModulePin = 2, Major = IOMajor.Light, IOGroupId = 2, Description = "Faretti" },

                // Queste 4 righe mappano 2 tapparelle (le tapparelle hanno il comando up e down e sono legate tramite ParentId)
                new IO() { Id = 16, DFCPAddress = 1491, ModulePin = 1, Major = IOMajor.Shutter, IOGroupId = 8, Description = "Tapparella", ParentId = 17 },       // bagno camera
                new IO() { Id = 17, DFCPAddress = 1491, ModulePin = 2, Major = IOMajor.ShutterUp, IOGroupId = 7, Description = "Tapparella up", ParentId = 16 },  // camera
                new IO() { Id = 18, DFCPAddress = 1491, ModulePin = 3, Major = IOMajor.Shutter, IOGroupId = 7, Description = "Tapparella", ParentId = 19 },       // camera
                new IO() { Id = 19, DFCPAddress = 1491, ModulePin = 0, Major = IOMajor.ShutterUp, IOGroupId = 8, Description = "Tapparella up", ParentId = 18 },  // bagno camera

                // Queste 4 righe mappano 4 zone di irrigazione 
                new IO() { Id = 43, DFCPAddress = 1503, ModulePin = 0, Major = IOMajor.Sprinkler, IOGroupId = 9, DefaultIsActive = false, Description = "Zona inquilini lato box" },
                new IO() { Id = 44, DFCPAddress = 1503, ModulePin = 1, Major = IOMajor.Sprinkler, IOGroupId = 9, DefaultIsActive = false, Description = "Zona lato via venezia" },
                new IO() { Id = 45, DFCPAddress = 1503, ModulePin = 2, Major = IOMajor.Sprinkler, IOGroupId = 9, DefaultIsActive = false, Description = "Zona fronte veranda" },
                new IO() { Id = 46, DFCPAddress = 1503, ModulePin = 3, Major = IOMajor.Sprinkler, IOGroupId = 9, DefaultIsActive = false, Description = "Zona inquilini lato cancello" },

                // Questa riga mappa il sensore di pioggia (utile per non attivare l'irrigazione se ha piovuto)
                new IO() { Id = 107, DFCPAddress = 479, ModulePin = 0, Major = IOMajor.RainSensor, IOGroupId = 9, IsInverted = true, Description = "Sensore pioggia" },

                // Queste 7 righe mappano le elettrovalvole per controllare il riscaldamento camera per camera
                new IO() { Id = 55, DFCPAddress = 1115, ModulePin = 0, Major = IOMajor.Valve, IOGroupId = 2, DefaultIsActive = false, Description = "Elettrovalvola soggiorno" },
                new IO() { Id = 56, DFCPAddress = 1115, ModulePin = 1, Major = IOMajor.Valve, IOGroupId = 1, DefaultIsActive = false, Description = "Elettrovalvola cucina" },
                new IO() { Id = 57, DFCPAddress = 1115, ModulePin = 2, Major = IOMajor.Valve, IOGroupId = 4, DefaultIsActive = false, Description = "Elettrovalvola studio" },
                new IO() { Id = 58, DFCPAddress = 1115, ModulePin = 3, Major = IOMajor.Valve, IOGroupId = 5, DefaultIsActive = false, Description = "Elettrovalvola cameretta" },
                new IO() { Id = 59, DFCPAddress = 1115, ModulePin = 4, Major = IOMajor.Valve, IOGroupId = 6, DefaultIsActive = false, Description = "Elettrovalvola bagno" },
                new IO() { Id = 60, DFCPAddress = 1115, ModulePin = 5, Major = IOMajor.Valve, IOGroupId = 7, DefaultIsActive = false, Description = "Elettrovalvola camera" },
                new IO() { Id = 61, DFCPAddress = 1115, ModulePin = 6, Major = IOMajor.Valve, IOGroupId = 8, DefaultIsActive = false, Description = "Elettrovalvola bagno camera" },

                // Questa riga mappa una presa elettrica
                new IO() { Id = 70, DFCPAddress = 1461, ModulePin = 1, Major = IOMajor.Socket, IOGroupId = 9, Description = "Prese esterne" },

                // Questa riga mappa 4 sensori di apertura finestra
                new IO() { Id = 62, DFCPAddress = 117, ModulePin = 0, Major = IOMajor.DoorSensor, IOGroupId = 1, IsInverted = true, Description = "Porta finestra cucina" },
                new IO() { Id = 63, DFCPAddress = 117, ModulePin = 1, Major = IOMajor.VasistasSensor, IOGroupId = 1, IsInverted = true, Description = "Porta finestra cucina vasistas" },
                new IO() { Id = 72, DFCPAddress = 455, ModulePin = 0, Major = IOMajor.VasistasSensor, IOGroupId = 1, IsInverted = true, Description = "Finestra lavello SX vasistas" },
                new IO() { Id = 74, DFCPAddress = 119, ModulePin = 0, Major = IOMajor.WindowSensor, IOGroupId = 1, IsInverted = true, Description = "Finestra piccola" },

                // Questa riga mappa i pulsanti di SOS (in questo caso la corda posizionata sopra la doccia)
                new IO() { Id = 76, DFCPAddress = 431, ModulePin = 3, Major = IOMajor.EmergencyButton, IOGroupId = 6, Description = "Emergenza bagno" },

                // Queste 7 righe mappano i sensori di temperatura DUEMMEGI posizionati in ogni stanza 
                // Tramite la proprietà Correction è possibile apportare una correzione nel caso il sensore sia sfasato
                new IO() { Id = 78, DFCPAddress = 12, ModulePin = 0, Major = IOMajor.TemperatureSensor, IOGroupId = 1, Description = "Temperatura cucina" },
                new IO() { Id = 79, DFCPAddress = 22, ModulePin = 0, Major = IOMajor.TemperatureSensor, IOGroupId = 2, Description = "Temperatura salotto", Correction = -1.5f },
                new IO() { Id = 80, DFCPAddress = 32, ModulePin = 0, Major = IOMajor.TemperatureSensor, IOGroupId = 4, Description = "Temperatura studio" },
                new IO() { Id = 81, DFCPAddress = 42, ModulePin = 0, Major = IOMajor.TemperatureSensor, IOGroupId = 5, Description = "Temperatura cameretta" },
                new IO() { Id = 82, DFCPAddress = 52, ModulePin = 0, Major = IOMajor.TemperatureSensor, IOGroupId = 6, Description = "Temperatura bagno" },
                new IO() { Id = 83, DFCPAddress = 62, ModulePin = 0, Major = IOMajor.TemperatureSensor, IOGroupId = 7, Description = "Temperatura camera", Correction = -0.9f },
                new IO() { Id = 84, DFCPAddress = 72, ModulePin = 0, Major = IOMajor.TemperatureSensor, IOGroupId = 8, Description = "Temperatura bagno camera" },

                // Queste 4 righe mappano i sensori antifumo
                new IO() { Id = 110, DFCPAddress = 117, ModulePin = 3, Major = IOMajor.SmokeSensor, IOGroupId = 1, IsInverted = true, Description = "Antifumo area tavolo" },
                new IO() { Id = 111, DFCPAddress = 119, ModulePin = 3, Major = IOMajor.SmokeSensor, IOGroupId = 1, IsInverted = true, Description = "Antifumo area fornelli" },
                new IO() { Id = 112, DFCPAddress = 121, ModulePin = 2, Major = IOMajor.SmokeSensor, IOGroupId = 4, IsInverted = true, Description = "Antifumo cameretta" },
                new IO() { Id = 113, DFCPAddress = 121, ModulePin = 3, Major = IOMajor.SmokeSensor, IOGroupId = 3, IsInverted = true, Description = "Antifumo studio" },

                // Queste 4 righe mappano sirene antifurto e combinatore GSM
                new IO() { Id = 116, DFCPAddress = 1185, ModulePin = 0, Major = IOMajor.Siren, IOGroupId = 9, DefaultIsActive = false, Description = "Sirena esterna" },
                new IO() { Id = 117, DFCPAddress = 1185, ModulePin = 1, Major = IOMajor.InternalSiren, IOGroupId = 2, DefaultIsActive = false, Description = "Sirena interna" },
                new IO() { Id = 118, DFCPAddress = 1185, ModulePin = 2, Major = IOMajor.Dialer, IOGroupId = 2, DefaultIsActive = false, Description = "Dialer: SOS" },
                new IO() { Id = 119, DFCPAddress = 1185, ModulePin = 3, Major = IOMajor.Dialer, IOGroupId = 2, DefaultIsActive = false, Description = "Dialer: antifurto e incendio" }

           );

            ///
            /// QUI VANNO INSERITE LE ZONE DI RISCALDAMENTO. 
            /// OGNI ZONA E' COMPOSTA DA UN SENSORE TEMPERATURA E UNA VALVOLA
            ///
            modelBuilder.Entity<Heating>().HasData(
                new Heating() { Id = 1, InputSensorId = 78, OutputValveId = 56, Temperature = 19, Description = "Cucina" },
                new Heating() { Id = 2, InputSensorId = 79, OutputValveId = 55, Temperature = 19, Description = "Salotto" },
                new Heating() { Id = 3, InputSensorId = 80, OutputValveId = 57, Temperature = 19, Description = "Studio" },
                new Heating() { Id = 4, InputSensorId = 81, OutputValveId = 58, Temperature = 19, Description = "Cameretta" },
                new Heating() { Id = 5, InputSensorId = 82, OutputValveId = 59, Temperature = 19, Description = "Bagno" },
                new Heating() { Id = 6, InputSensorId = 83, OutputValveId = 60, Temperature = 19, Description = "Camera" },
                new Heating() { Id = 7, InputSensorId = 84, OutputValveId = 61, Temperature = 19, Description = "Bagno camera" }
            );

            modelBuilder.Entity<HeatingSetting>().HasData(
                new HeatingSetting() { Id = 1, Mode = HeatingMode.Disabled, Offset = 0.5f }
            );

            modelBuilder.Entity<HeatingTime>().HasData(
                new HeatingTime() { Id = 1, HeatingSettingId = 1, StartTime = new TimeSpan(6, 0, 0), EndTime = new TimeSpan(10, 0, 0) },
                new HeatingTime() { Id = 2, HeatingSettingId = 1, StartTime = new TimeSpan(16, 0, 0), EndTime = new TimeSpan(20, 0, 0) }
            );

            ///
            /// QUI VANNO INSERITE LE ZONE DI IRRIGAZIONE. 
            ///
            modelBuilder.Entity<Irrigation>().HasData(
                new Irrigation() { Id = 1, OutputSprinklerId = 43, Minutes = 7, Description = "Irrigazione ingresso inquilini lato box" },
                new Irrigation() { Id = 2, OutputSprinklerId = 44, Minutes = 7, Description = "Irrigazione lato via venezia" },
                new Irrigation() { Id = 3, OutputSprinklerId = 45, Minutes = 7, Description = "Irrigazione fronte veranda" },
                new Irrigation() { Id = 4, OutputSprinklerId = 46, Minutes = 7, Description = "Irrigazione ingresso inquilini lato cancello" }
            );

            modelBuilder.Entity<IrrigationSetting>().HasData(
                // setto LastExecutionDate alla data attuale altrimenti l'irrigazione parte immediatamente
                new IrrigationSetting() { Id = 1, Mode = IrrigationMode.Auto, StartTime = new TimeSpan(2, 0, 0), LastExecution = DateTime.Now.Date, RainSensorId = 107, Monday = true, Tuesday = true, Wednesday = true, Thursday = true, Friday = true, Saturday = true, Sunday = true }
            );

            modelBuilder.Entity<SecuritySetting>().HasData(
                new SecuritySetting() { Id = 1, AntiFire = true, AntiFireDetail = string.Empty, SOSDetail = string.Empty, AntitheftDetail = string.Empty, AntitheftMode = SecurityAntitheftMode.Disabled }
            );

            ///
            /// GLI SCHEDULER PERMETTO DI AVVIARE DELLE AUTOMAZIONE
            /// PER ESEMPIO ACCENDERE E SPEGNERE LUCI E PRESE A UNA DETERMINATA DATA/ORA
            /// QUESTA FUNZIONE E' IN BETA E NON ESISTE UNA GUI PER GESTIRLA
            ///

            modelBuilder.Entity<Scheduler>().HasData(
                new Scheduler() { Id = 1, Mode = SchedulerMode.Disabled, Start = new DateTime(2022, 12, 1, 17, 0, 0), End = new DateTime(2023, 1, 20, 2, 0, 0), Description = "Luci natale" }
            );

            modelBuilder.Entity<SchedulerItem>().HasData(
                new SchedulerItem() { Id = 1, SchedulerId = 1, OutputId = 70 }
            );

            ///
            /// Un utente è obbligatorio, l'hash della password può essere generato usando SecurityHelper.GenerateSHA256
            ///
            modelBuilder.Entity<User>().HasData(
                new User() { Id = 1, Name = "[NAME]", Surname = "[SURNAME]", Mail = "[MAIL]", PasswordHash = "[PASSWORD-HASH]", NotifyMail = true }
            );

            modelBuilder.Entity<NotifySetting>().HasData(
                new NotifySetting() { Id = 1,  DialerAutomaticId = 119, DialerSOSId = 118 }
            );
        }
    }
}
