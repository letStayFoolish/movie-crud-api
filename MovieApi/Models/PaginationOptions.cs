// This file is part of the project. Copyright (c) Company.

using System.ComponentModel.DataAnnotations;

namespace MovieApi.Models;

public class PaginationOptions
{
    public int Page { get; set; }
    [Required, Range(1, 100)] public int PageSize { get; set; }
    [Required, Range(1, 100)]
    public int MaxPageSize { get; set; }
}
