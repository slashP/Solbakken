using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using CodeFirstMembershipSharp;

namespace Solbakken.Models
{
    public class Bilde
    {
        public int Id { get; set; }
        public string Navn { get; set; }
        public string Beskrivelse { get; set; }
        public Guid LastetOppAvId { get; set; }
        public virtual User LastetOppAv { get; set; }
        [Required(ErrorMessage = "Du må velge et album. Opprett et først hvis du ikke finner ett.")]
        public int? AlbumId { get; set; }
        public virtual Album Album { get; set; }
        public byte[] BildeStream { get; set; }
        public byte[] Thumbnail { get; set; }
        public string Filnavn { get; set; }
        public string Format { get; set; }
    }
}