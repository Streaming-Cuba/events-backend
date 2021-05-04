using System;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Events.API.Helpers
{
    public static class JsonPatchExtensions
    {
        public static void ApplyTo<TModelDTO, TModel>(
            this JsonPatchDocument<TModelDTO> patchDTO,
            TModel model,
            IMapper mapper,
            ModelStateDictionary modelState,
            Action<TModelDTO> action = null) where TModelDTO : class
        {
            var dto = mapper.Map<TModelDTO>(model);
            patchDTO.ApplyTo(dto, modelState);
            action?.Invoke(dto);
            if (modelState.IsValid)
                model = mapper.Map<TModelDTO, TModel>(dto, model);
        }
    }
}