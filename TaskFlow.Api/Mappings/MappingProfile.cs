using AutoMapper;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Categories
            //CreateMap<Nguồn, Đích>
            //xuat tu category qua categories
            CreateMap<Category, CategoriesResponseDto>()
            .ForMember(dest => dest.TotalItem, opt => opt.MapFrom(src => src.TodoItems.Count));

            //xuat tu dto sang category
            CreateMap<CreateCategoryRequestDto, Category>();

            CreateMap<UpdateCategoryRequestDto, Category>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));



            //ToDoItems
            CreateMap<TodoItem, TodoItemResponseDto>()
                .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "N/A"));

            // 2. Chiều Nhập để Tạo mới (Create DTO -> Model)
            CreateMap<CreateTodoItemRequestDto, TodoItem>();

            // 3. Chiều Nhập để Cập nhật (Update DTO -> Model) + Chặn đè NULL
            CreateMap<UpdateTodoItemRequestDto, TodoItem>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
