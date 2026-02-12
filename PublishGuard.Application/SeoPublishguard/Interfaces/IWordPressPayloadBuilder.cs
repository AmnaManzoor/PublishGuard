using SEO.Publishguard.Domain;

namespace SEO.Publishguard.Application;

public interface IWordPressPayloadBuilder
{
    WordPressPayload Build(Article article);
}
