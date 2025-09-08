using LexMigración.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LexMigración.Services
{
    public class DatabaseService
    {
        // --- Constructor ---
        // Se asegura de que la base de datos y las tablas se creen al iniciar el servicio.
        public DatabaseService()
        {
            try
            {
                using (var db = new LexMigracionContext())
                {
                    db.Database.EnsureCreated();
                }
            }
            catch (Exception ex)
            {
                // Si algo sale mal, nos mostrará un mensaje de error detallado.
                MessageBox.Show($"Error al crear o conectar a la base de datos: {ex.Message}");
            }
        }

        // --- Métodos para Anexos ---
        public List<Anexo> ObtenerAnexos()
        {
            using (var db = new LexMigracionContext())
            {
                return db.Anexos.ToList();
            }
        }

        public void GuardarAnexo(Anexo anexo)
        {
            using (var db = new LexMigracionContext())
            {
                db.Anexos.Add(anexo);
                db.SaveChanges(); // Guarda los cambios en la base de datos
            }
        }

        // --- Métodos para Protocolos ---
        public List<ProtocoloModel> ObtenerProtocolos()
        {
            using (var db = new LexMigracionContext())
            {
                return db.Protocolos.ToList();
            }
        }

        public void GuardarProtocolo(ProtocoloModel protocolo)
        {
            using (var db = new LexMigracionContext())
            {
                db.Protocolos.Add(protocolo);
                db.SaveChanges();
            }
        }

        // --- Métodos para Índice ---
        public List<RegistroIndice> ObtenerRegistrosIndice()
        {
            using (var db = new LexMigracionContext())
            {
                return db.Indices.ToList();
            }
        }

        public void GuardarRegistroIndice(RegistroIndice registro)
        {
            using (var db = new LexMigracionContext())
            {
                db.Indices.Add(registro);
                db.SaveChanges();
            }
        }
    }
}