namespace MovieApi.Models;

public class Movie : EntityBase
{
    // private setters to ensure that changes can only be made within the class, maintaining the integrity of the object.
    public string Title { get; private set; }
    public string Genre { get; private set; }
    public DateTimeOffset ReleaseDate { get; private set; }
    public double Rating { get; private set; }
    
    // private constructor for ORM frameworks
    private Movie()
    {
        this.Title = string.Empty;
        this.Genre = string.Empty;
    }

    private Movie(string title, string genre, DateTimeOffset releaseDate, double rating)
    {
        this.Title = title;
        this.Genre = genre;
        this.ReleaseDate = releaseDate;
        this.Rating = rating;
    }

    public static Movie Create(string title, string genre, DateTimeOffset releaseDate, double rating)
    {
        ValidateInputs(title, genre, releaseDate, rating);
        
        return new Movie()
        {
            Title = title,
            Genre = genre,
            ReleaseDate = releaseDate,
            Rating = rating
        };
    }

    public void Update(string title, string genre, DateTimeOffset releaseDate, double rating)
    {
        ValidateInputs(title, genre, releaseDate, rating);
        this.Title = title;
        this.Genre = genre;
        this.ReleaseDate = releaseDate;
        this.Rating = rating;
        
        // Update
        UpdateLastModified();
    }

    private static void ValidateInputs(string title, string genre, DateTimeOffset releaseDate, double rating)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null or whitespace", nameof(title));
        }
        if (string.IsNullOrWhiteSpace(genre))
        {
            throw new ArgumentException("Genre cannot be null or whitespace", nameof(genre));
        }
        if (releaseDate < DateTimeOffset.MinValue)
        {
            throw new ArgumentException("Release date cannot be before 1900", nameof(releaseDate));
        }
        if (rating < 0 || rating > 10)
        {
            throw new ArgumentException("Rating must be between 0 and 10", nameof(rating));
        }
    }
}