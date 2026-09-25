namespace SocialHub.Api.Interfaces;

public interface IMetaOAuthStateStore
{
    string Create(int workspaceId);
    bool TryConsume(string state, out int workspaceId);
}
