using System.Collections.Generic;

public enum BTStatus { Success, Failure, Running }

public abstract class BTNode {
    public abstract BTStatus Tick(EnnemiBTContext ctx);
}

public sealed class BTSequence : BTNode {
    readonly List<BTNode> _children = new List<BTNode>();
    int _index;
    public BTSequence(params BTNode[] nodes) { _children.AddRange(nodes); }

    public override BTStatus Tick(EnnemiBTContext ctx) {
        while (_index < _children.Count) {
            var s = _children[_index].Tick(ctx);
            if (s == BTStatus.Running) return BTStatus.Running;
            if (s == BTStatus.Failure) { _index = 0; return BTStatus.Failure; }
            _index++;
        }
        _index = 0;
        return BTStatus.Success;
    }
}

public sealed class BTSelector : BTNode {
    readonly List<BTNode> _children = new List<BTNode>();
    int _index;
    public BTSelector(params BTNode[] nodes) { _children.AddRange(nodes); }

    public override BTStatus Tick(EnnemiBTContext ctx) {
        while (_index < _children.Count) {
            var s = _children[_index].Tick(ctx);
            if (s == BTStatus.Running) return BTStatus.Running;
            if (s == BTStatus.Success) { _index = 0; return BTStatus.Success; }
            _index++;
        }
        _index = 0;
        return BTStatus.Failure;
    }
}