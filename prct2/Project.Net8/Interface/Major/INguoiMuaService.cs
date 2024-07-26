using DTC.DefaultRepository.FromBodyModels;
using Project.Net8.Models.Major;
using Project.Net8.Models.PagingParam;

namespace Project.Net8.Interface.Major
{
    public interface INguoiMuaService
    {
        Task<dynamic> Create(NguoiMuaModel model);
        Task<dynamic> Update(NguoiMuaModel model);

    }
}