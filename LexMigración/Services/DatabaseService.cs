using LexMigración.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LexMigración.Services
{
    public class DatabaseService
    {
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
                MessageBox.Show($"Error al conectar con la base de datos: {ex.Message}");
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
                db.SaveChanges();
            }
        }

        public void ActualizarAnexo(Anexo anexo)
        {
            using (var db = new LexMigracionContext())
            {
                db.Anexos.Update(anexo);
                db.SaveChanges();
            }
        }

        public void EliminarAnexo(Anexo anexo)
        {
            using (var db = new LexMigracionContext())
            {
                db.Anexos.Remove(anexo);
                db.SaveChanges();
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

        // --- Métodos para Expedientes ---
        public List<Expediente> ObtenerExpedientes()
        {
            using (var db = new LexMigracionContext())
            {
                return db.Expedientes.ToList();
            }
        }

        // --- Métodos para Usuarios (Login y Registro) ---
        public bool ValidarUsuario(string nombreUsuario, string contrasena, string rol)
        {
            using (var db = new LexMigracionContext())
            {
                var usuario = db.Usuarios.FirstOrDefault(u =>
                    u.NombreUsuario == nombreUsuario &&
                    u.Contrasena == contrasena &&
                    u.Rol == rol);

                return usuario != null;
            }
        }

        public string RegistrarUsuario(Usuario nuevoUsuario)
        {
            using (var db = new LexMigracionContext())
            {
                if (db.Usuarios.Any(u => u.NombreUsuario == nuevoUsuario.NombreUsuario))
                {
                    return "El nombre de usuario ya está en uso. Por favor, elige otro.";
                }

                db.Usuarios.Add(nuevoUsuario);
                db.SaveChanges();

                return "Éxito";
            }
        }
    }
}