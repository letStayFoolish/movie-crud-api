using MovieApi.Models;

namespace MovieApi.Tests;

public class MovieTests
{
    [Fact]
    public void Create_WithValidInputs_ShouldReturnMovie()
    {
        // Arrange
        var title = "Inception";
        var genre = "Sci-Fi";
        var releaseDate = new DateTimeOffset(2010, 7, 16, 0, 0, 0, TimeSpan.Zero);
        var rating = 8.8;

        // Act
        var movie = Movie.Create(title, genre, releaseDate, rating);

        // Assert
        Assert.NotNull(movie);
        Assert.Equal(title, movie.Title);
        Assert.Equal(genre, movie.Genre);
        Assert.Equal(releaseDate, movie.ReleaseDate);
        Assert.Equal(rating, movie.Rating);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidTitle_ShouldThrowArgumentException(string? invalidTitle)
    {
        // Arrange
        var genre = "Sci-Fi";
        var releaseDate = DateTimeOffset.Now;
        var rating = 8.8;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Movie.Create(invalidTitle!, genre, releaseDate, rating));

        Assert.Contains("Title cannot be null or whitespace", exception.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(10.1)]
    public void Create_WithInvalidRating_ShouldThrowArgumentException(double invalidRating)
    {
        // Arrange
        var title = "Inception";
        var genre = "Sci-Fi";
        var releaseDate = DateTimeOffset.Now;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Movie.Create(title, genre, releaseDate, invalidRating));

        Assert.Contains("Rating must be between 0 and 10", exception.Message);
    }

    [Fact]
    public void Update_WithValidInputs_ShouldUpdateMovieProperties()
    {
        // Arrange
        var movie = Movie.Create("Original Title", "Original Genre", DateTimeOffset.Now, 5.0);
        var newTitle = "Updated Title";
        var newGenre = "Updated Genre";
        var newReleaseDate = DateTimeOffset.Now.AddDays(1);
        var newRating = 9.0;

        // Act
        movie.Update(newTitle, newGenre, newReleaseDate, newRating);

        // Assert
        Assert.Equal(newTitle, movie.Title);
        Assert.Equal(newGenre, movie.Genre);
        Assert.Equal(newReleaseDate, movie.ReleaseDate);
        Assert.Equal(newRating, movie.Rating);
    }
}
