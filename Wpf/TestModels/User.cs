﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TestModels
{
    [DataContract]
    public class User : IdentityUser
    {
        [DataMember]
        public override string Id { get; set; }

        [DataMember]
        public string FIO { get; set; }

        [DataMember]
        [Required]
        public override string UserName { get; set; }

        [DataMember]
        [Required]
        public override string PasswordHash { get; set; }

        [DataMember]
        public override string SecurityStamp { get; set; }

        [DataMember]
        public override string Email { get; set; }

        [DataMember]
        public int? UserGroupId { get; set; }

        public virtual UserGroup UserGroup { get; set; }

        [ForeignKey("UserId")]
        public virtual List<Stat> Stats { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Здесь добавьте настраиваемые утверждения пользователя
            return userIdentity;
        }
    }
}
