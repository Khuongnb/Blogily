namespace Conduit.Infrastructure.CLParser.Comments
{
    public interface ICommentsActionHandler
    {
        // create-comment
        int CreateComment(CreateCommentOption opt);

        // list-comments
        int ListComments(ListCommentsOption opt);

        // delete-comment
        int DeleteComment(DeleteCommentOption opt);
    }
}
