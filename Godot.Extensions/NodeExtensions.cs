namespace Godot.Extensions;

public static class NodeExtensions
{
    public static TNode? GetNode<TNode>(this Node self, NodePath? nodePath, ref TNode? node) where TNode : Node
    {
        if (node != default || nodePath is null)
        {
            return node;
        }

        node = self.GetNode<TNode>(nodePath);
        return node;
    }
}