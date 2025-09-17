// This file is part of the project. Copyright (c) Company.

using System.ComponentModel.DataAnnotations;

namespace MovieApi.Models;

public class PaginationOptions
{
    public int Page { get; set; } = default;
    [Required, Range(1, 100)] public int PageSize { get; set; } = 10;
    [Required, Range(1, 100)]
    public int MaxPageSize { get; set; }
}
