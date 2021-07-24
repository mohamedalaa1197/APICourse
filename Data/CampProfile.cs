using AutoMapper;
using Pluralsight.Data.Entities;
using Pluralsight.Models;

namespace Pluralsight.Data
{
    public class CampProfile:Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp,CampModel>()
                .ForMember(c=>c.Venue,o=>o.MapFrom(m=>m.Location.VenueName))
                .ForMember(c=>c.add01,o=>o.MapFrom(m=>m.Location.Address1))
                .ForMember(c=>c.add02,o=>o.MapFrom(m=>m.Location.Address2))
                .ForMember(c=>c.add03,o=>o.MapFrom(m=>m.Location.Address3))
                .ForMember(c=>c.cityTown,o=>o.MapFrom(m=>m.Location.CityTown))
                .ForMember(c=>c.postalCode,o=>o.MapFrom(m=>m.Location.PostalCode))
                .ForMember(c=>c.stateProvider,o=>o.MapFrom(m=>m.Location.StateProvince))
                .ReverseMap()
            ;

            this.CreateMap<Talk,TalkModel>()
            .ReverseMap()
            .ForMember(t=>t.Camp,opt=>opt.Ignore())
            .ForMember(s=>s.Speaker,opt=>opt.Ignore());

            this.CreateMap<Speaker,SpeakerModel>()
            .ReverseMap()
         
            ;
        }
    }
}