using Microsoft.Extensions.Options;
using SEO.Publishguard.Application;
using SEO.Publishguard.Domain;
using SEO.Publishguard.Infrastructure;

namespace PublishGuard.Tests.SeoPublishguard;

public sealed class ValidationRulesTests
{
    [Fact]
    public void ImageCountValidationRule_ReturnsIssueWhenBelowMinimum()
    {
        var options = Options.Create(new ValidationRuleOptions { MinImages = 2, MaxImages = 4 });
        var rule = new ImageCountValidationRule(options);
        var article = CreateArticle(images: new[] { CreateImage() });

        var issue = rule.Validate(article);

        Assert.NotNull(issue);
        Assert.Equal(ValidationIssueCode.ImagesTooFew, issue?.Code);
    }

    [Fact]
    public void ImageCountValidationRule_ReturnsIssueWhenAboveMaximum()
    {
        var options = Options.Create(new ValidationRuleOptions { MinImages = 1, MaxImages = 2 });
        var rule = new ImageCountValidationRule(options);
        var article = CreateArticle(images: new[] { CreateImage(), CreateImage(), CreateImage() });

        var issue = rule.Validate(article);

        Assert.NotNull(issue);
        Assert.Equal(ValidationIssueCode.ImagesTooMany, issue?.Code);
    }

    [Fact]
    public void ProductLinkValidationRule_ReturnsIssueWhenBelowMinimum()
    {
        var options = Options.Create(new ValidationRuleOptions { MinProductLinks = 2, MaxProductLinks = 4 });
        var rule = new ProductLinkValidationRule(options);
        var article = CreateArticle(links: new[] { CreateLink(isProduct: true) });

        var issue = rule.Validate(article);

        Assert.NotNull(issue);
        Assert.Equal(ValidationIssueCode.ProductLinksTooFew, issue?.Code);
    }

    [Fact]
    public void H2CountValidationRule_ReturnsIssueWhenBelowMinimum()
    {
        var options = Options.Create(new ValidationRuleOptions { MinH2Headings = 3 });
        var rule = new H2CountValidationRule(options);
        var article = CreateArticle(h2Count: 1);

        var issue = rule.Validate(article);

        Assert.NotNull(issue);
        Assert.Equal(ValidationIssueCode.MissingH2Headings, issue?.Code);
    }

    [Fact]
    public void BoldPercentageValidationRule_ReturnsIssueWhenAboveMaximum()
    {
        var options = Options.Create(new ValidationRuleOptions { MaxBoldPercentage = 10 });
        var rule = new BoldPercentageValidationRule(options);
        var article = CreateArticle(boldPercentage: 25);

        var issue = rule.Validate(article);

        Assert.NotNull(issue);
        Assert.Equal(ValidationIssueCode.ExcessiveBoldText, issue?.Code);
    }

    [Fact]
    public void GoogleDriveImageValidationRule_ReturnsIssueWhenImageNotHostedOnDrive()
    {
        var rule = new GoogleDriveImageValidationRule();
        var images = new[]
        {
            new ImageInfo("https://cdn.example.com/image.jpg", "Alt", false, true)
        };
        var article = CreateArticle(images: images);

        var issue = rule.Validate(article);

        Assert.NotNull(issue);
        Assert.Equal(ValidationIssueCode.ImageNotOnGoogleDrive, issue?.Code);
    }

    [Fact]
    public void GoogleDriveImageValidationRule_ReturnsIssueWhenImageNotPublic()
    {
        var rule = new GoogleDriveImageValidationRule();
        var images = new[]
        {
            new ImageInfo("https://drive.google.com/file/d/123", "Alt", true, false)
        };
        var article = CreateArticle(images: images);

        var issue = rule.Validate(article);

        Assert.NotNull(issue);
        Assert.Equal(ValidationIssueCode.ImageNotPublic, issue?.Code);
    }

    private static Article CreateArticle(
        IReadOnlyList<ImageInfo>? images = null,
        IReadOnlyList<LinkInfo>? links = null,
        int wordCount = 100,
        int h2Count = 2,
        decimal boldPercentage = 5)
    {
        return new Article(
            "https://docs.google.com/document/d/123",
            "Sample",
            "<p>Body</p>",
            images ?? Array.Empty<ImageInfo>(),
            links ?? Array.Empty<LinkInfo>(),
            wordCount,
            h2Count,
            boldPercentage);
    }

    private static ImageInfo CreateImage()
    {
        return new ImageInfo("https://drive.google.com/file/d/123", "Alt", true, true);
    }

    private static LinkInfo CreateLink(bool isProduct)
    {
        return new LinkInfo("https://example.com/products/123", "Link", isProduct);
    }
}
