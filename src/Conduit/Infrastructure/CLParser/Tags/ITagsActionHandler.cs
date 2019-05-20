namespace Conduit.Infrastructure.CLParser.Tags
{
    public interface ITagsActionHandler
    {

        // List all tags: list-tags
        int ListTags(ListTagsOption opt);
    }
}
