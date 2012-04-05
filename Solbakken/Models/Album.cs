using System;
using System.Collections.ObjectModel;
using CodeFirstMembershipSharp;

namespace Solbakken.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string Navn { get; set; }
        public DateTime Opprettet { get; set; }
        public User OpprettetAv { get; set; }
        public virtual Collection<Bilde> Bilder { get; set; }
    }
}