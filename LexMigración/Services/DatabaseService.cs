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
                    // Esto crea el archivo .db si no existe y define el esquema (las tablas).
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

        /// <summary>
        /// Obtiene una lista de todos los anexos de la base de datos.
        /// </summary>
        public List<Anexo> ObtenerAnexos()
        {
            using (var db = new LexMigracionContext())
            {
                return db.Anexos.ToList();
            }
        }

        /// <summary>
        /// Guarda un nuevo anexo en la base de datos.
        /// </summary>
        public void GuardarAnexo(Anexo anexo)
        {
            using (var db = new LexMigracionContext())
            {
                db.Anexos.Add(anexo);
                db.SaveChanges(); // Guarda los cambios
            }
        }

        /// <summary>
        /// Actualiza un anexo existente en la base de datos.
        /// </summary>
        public void ActualizarAnexo(Anexo anexo)
        {
            using (var db = new LexMigracionContext())
            {
                db.Anexos.Update(anexo);
                db.SaveChanges(); // Guarda los cambios
            }
        }


        // --- Métodos para Protocolos ---

        /// <summary>
        /// Obtiene una lista de todos los protocolos de la base de datos.
        /// </summary>
        public List<ProtocoloModel> ObtenerProtocolos()
        {
            using (var db = new LexMigracionContext())
            {
                return db.Protocolos.ToList();
            }
        }

        /// <summary>
        /// Guarda un nuevo protocolo en la base de datos.
        /// </summary>
        public void GuardarProtocolo(ProtocoloModel protocolo)
        {
            using (var db = new LexMigracionContext())
            {
                db.Protocolos.Add(protocolo);
                db.SaveChanges();
            }
        }


        // --- Métodos para Índice ---

        /// <summary>
        /// Obtiene una lista de todos los registros del índice de la base de datos.
        /// </summary>
        public List<RegistroIndice> ObtenerRegistrosIndice()
        {
            using (var db = new LexMigracionContext())
            {
                return db.Indices.ToList();
            }
        }

        /// <summary>
        /// Guarda un nuevo registro de índice en la base de datos.
        /// </summary>
        public void GuardarRegistroIndice(RegistroIndice registro)
        {
            using (var db = new LexMigracionContext())
            {
                // Lógica para determinar si es un registro nuevo o uno existente
                if (registro.Id > 0)
                {
                    db.Indices.Update(registro);
                }
                else
                {
                    db.Indices.Add(registro);
                }
                db.SaveChanges();
            }
        }
    }
}