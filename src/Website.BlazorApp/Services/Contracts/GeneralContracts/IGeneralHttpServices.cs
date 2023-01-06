

using Website.BlazorApp.Models.GeneralModels;

namespace Website.BlazorApp.Services.Contracts.GeneralContracts;

public interface IGeneralHttpServices
{
    Task<ResultMessage<IEnumerable<T>>> GetPaginated<T>(string address, int? pageIndex = null, int? pageSize = null, string? searchTerm = null, string? orderByPropertyName = null, bool desc = false);
    Task<T> GetOne<T>(string address);
     Task<bool> Add<T>(object viewModel, string address);
     Task<bool> Update<T>(object viewModel, string address);
     Task<bool> Update(string address);
    Task<bool> Delete(string address);
}