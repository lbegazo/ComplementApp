using System;

namespace ComplementApp.API.Dtos
{
    public class PhotoForDetailedDto
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public DateTime DateAdded { get; set; }

        public string Description { get; set; }

        public bool IsMain { get; set; }
    }
}