// This file is part of the project. Copyright (c) Company.

using System.ComponentModel.DataAnnotations;

namespace MovieApi.Models;

public class PaginationOptions
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)] public int PageSize { get; set; } = 10;
    [Range(1, 100)] public int MaxPageSize { get; set; } = 100;
}
