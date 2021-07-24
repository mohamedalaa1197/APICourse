using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pluralsight.Data.Entities;

namespace Pluralsight.Models
{
    public class CampModel
    {
    public int CampId { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Moniker { get; set; }
    public DateTime EventDate { get; set; } = DateTime.MinValue;

    public string Venue { get; set; }
    public string add01 { get; set; }
    public string add02 { get; set; }
    public string add03 { get; set; }
    public string cityTown { get; set; }
    public string stateProvider { get; set; }
    public string postalCode { get; set; }
    public string LocationCountry { get; set; }

    public List<TalkModel> Talks { get; set; }
    }
}