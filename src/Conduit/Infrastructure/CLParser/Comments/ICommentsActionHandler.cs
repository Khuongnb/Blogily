namespace Conduit.Infrastructure.CLParser.Comments
{
    public interface ICommentsActionHandler
    {
        // add comment: create-comment --username="" --body="" --slug=""
        int CreateComment(CreateCommentOption opt);

        // get comments: list-comments --slug=""
        int ListComments(ListCommentsOption opt);

        // remove comment: delete-comment --slug="" --id=
        int DeleteComment(DeleteCommentOption opt);
    }
}
