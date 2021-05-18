using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Events.API.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Helpers
{
    public static class JsonPatchExtensions
    {
        public static async Task<TModelDTO> ApplyTo<TModelDTO, TModel>(this JsonPatchDocument<TModelDTO> patchDTO,
                                                                       TModel model,
                                                                       DbContext context,
                                                                       IMapper mapper,                                                                       
                                                                       ModelStateDictionary modelState,
                                                                       Action<TModelDTO> action = null) where TModelDTO : CreateModelDTO
        {
            var dto = mapper.Map<TModelDTO>(model);
            patchDTO.ApplyTo(dto, modelState);
            await dto.EnsureValidState(context, modelState);
            action?.Invoke(dto);
            if (modelState.IsValid)
                mapper.Map(dto, model);
            return dto;
        }
    }
}