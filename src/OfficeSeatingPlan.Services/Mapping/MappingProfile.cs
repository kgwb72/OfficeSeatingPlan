using AutoMapper;
using OfficeSeatingPlan.Core.DTOs;
using OfficeSeatingPlan.Core.Entities;

namespace OfficeSeatingPlan.Services.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.HasSeatAssigned, opt => opt.MapFrom(src => src.AssignedSeat != null));

        CreateMap<User, UserBasicDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        CreateMap<UserUpdateDto, User>();

        // Building mappings
        CreateMap<Building, BuildingDto>();
        CreateMap<BuildingDto, Building>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Layouts, opt => opt.Ignore());

        // Layout mappings
        CreateMap<Layout, LayoutDto>()
            .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.Building != null ? src.Building.Name : string.Empty));
        CreateMap<LayoutDto, Layout>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Building, opt => opt.Ignore())
            .ForMember(dest => dest.Furniture, opt => opt.Ignore())
            .ForMember(dest => dest.Walls, opt => opt.Ignore())
            .ForMember(dest => dest.Seats, opt => opt.Ignore());

        // Furniture mappings
        CreateMap<Furniture, FurnitureDto>()
            .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.Properties));
        CreateMap<FurnitureDto, Furniture>()
            .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.Properties))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Layout, opt => opt.Ignore());

        // Wall mappings
        CreateMap<Wall, WallDto>();
        CreateMap<WallDto, Wall>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Layout, opt => opt.Ignore());

        // Seat mappings
        CreateMap<Seat, SeatDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.Properties))
            .ForMember(dest => dest.AssignedUser, opt => opt.MapFrom(src => src.AssignedUser));
        CreateMap<SeatDto, Seat>()
            .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.Properties))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<SeatStatus>(src.Status)))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Layout, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedUser, opt => opt.Ignore())
            .ForMember(dest => dest.SeatAssignments, opt => opt.Ignore());

        CreateMap<Seat, SeatBasicDto>()
            .ForMember(dest => dest.LayoutName, opt => opt.MapFrom(src => src.Layout.Name))
            .ForMember(dest => dest.FloorNumber, opt => opt.MapFrom(src => src.Layout.FloorNumber))
            .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.Layout.Building.Name));

        // Seat Assignment mappings
        CreateMap<SeatAssignment, SeatAssignmentDto>()
            .ForMember(dest => dest.SeatIdentifier, opt => opt.MapFrom(src => src.Seat.Identifier))
            .ForMember(dest => dest.UserDisplayName, opt => opt.MapFrom(src => src.User.DisplayName))
            //.ForMember(dest => dest.AssignedByName, opt => opt.MapFrom(src => src.AssignedBy != null ? src.AssignedBy.DisplayName : null))
            ;

    }
}