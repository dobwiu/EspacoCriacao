using EcWebApp.DAL;
using EcWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EcWebApp.BLL
{
    public class Usuario
    {
        private EspacoContext db = new EspacoContext();

        public IEnumerable<UsuarioInfo> ListarUsuarioCombo()
        {
            Guid idAdmin = Guid.Empty;
            return db.Usuarios.Where(s => s.IdUsuario != idAdmin && s.Ativo).OrderBy(o => o.NomeUsuario);
        }

        public UsuarioInfo SelecionaUsuario(Guid pIdUsuario) {
            return db.Usuarios.Find(pIdUsuario);
        }

        #region Autenticação..
        public UsuarioInfo Autenticar(string usuario, string senha)
        {
            var usuarioInfo = db.Usuarios.Include("Permissoes").Where(s => s.LoginUsuario == usuario).FirstOrDefault();

            if (usuarioInfo != null)
            {
                string senhaMD5 = this.CalculateMD5Hash(senha);

                if (usuarioInfo.SenhaUsuario == senhaMD5 && usuarioInfo.Ativo)
                {
                    return usuarioInfo;
                }
            }

            return null;
        }

        public string RedefinirSenha(Guid pIdUsuario, string pNovaSenha)
        {
            var senhaMD5 = CalculateMD5Hash(pNovaSenha);
            var usuarioInfo = db.Usuarios.Find(pIdUsuario);

            if (usuarioInfo != null)
            {
                usuarioInfo.SenhaUsuario = senhaMD5;
                usuarioInfo.SenhaConfirmada = senhaMD5;

                db.Entry(usuarioInfo).State = EntityState.Modified;
                db.SaveChanges();
            }

            return senhaMD5;
        }

        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));

            return sb.ToString();
        }
        #endregion
    }
}