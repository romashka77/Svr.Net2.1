using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Web.Models.SubjectClaimsViewModels
{
    public class CreateViewModel : BaseEntityCode
    {
        public string StatusMessage { get; set; }
        /// <summary>
        /// Id Группы исков
        /// </summary>
        [Display(Name = "Группа исков")]
        public long GroupClaimId { get; set; }
        public override string ToString() => "Предмет иска";
    }
}
