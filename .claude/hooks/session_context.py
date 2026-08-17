#!/usr/bin/env python3
"""session_context.py — SessionStart hook: map health, injected once per session.

The skill tells Claude to trust the maps. This is what makes that trust
checkable: at session start it states, in facts and not in orders, which maps
are degraded and which are older than the files they describe. Roughly 1 KB of
context buys back every scan that a silently stale map would have caused.

Output is JSON with hookSpecificOutput.additionalContext (documented for
SessionStart). It is capped well under the 10,000-character hook limit, and it
is written as statements — a hook that issues instructions reads as injected
prompt text, which is not what this is for.

Usage: python3 session_context.py [project_root]   (reads the hook payload on stdin)
"""
import json
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

MAX_CHARS = 2000


def project_root() -> str:
    args = [a for a in sys.argv[1:] if not a.startswith("--")]
    if args:
        return os.path.abspath(args[0])
    env = os.environ.get("CLAUDE_PROJECT_DIR")
    if env:
        return os.path.abspath(env)
    try:
        payload = json.loads(sys.stdin.read() or "{}")
        if payload.get("cwd"):
            return os.path.abspath(payload["cwd"])
    except Exception:
        pass
    return os.getcwd()


def first_line(path):
    try:
        with open(path, encoding="utf-8") as f:
            return f.readline().strip()
    except OSError:
        return ""


def emit(lines):
    text = "\n".join(lines)
    if len(text) > MAX_CHARS:
        text = text[:MAX_CHARS - 3] + "..."
    print(json.dumps({"hookSpecificOutput": {
        "hookEventName": "SessionStart",
        "additionalContext": text,
    }}))


def main() -> int:
    root = project_root()
    cdir = os.path.join(root, ".claude")
    if not os.path.isdir(cdir):
        return 0   # not a unity-dev project; stay silent

    out = ["unity-dev map health at session start (facts, not instructions):"]

    # codemaps
    cm = []
    for fn in sorted(os.listdir(cdir)):
        if fn.startswith("codemap-") and fn.endswith(".md"):
            stamp = first_line(os.path.join(cdir, fn))
            m = re.search(r"status:\s*(.*?)\s*-->", stamp)
            status = m.group(1) if m else "no status field (pre-v2 stamp)"
            n = sum(1 for l in open(os.path.join(cdir, fn), encoding="utf-8")
                    if "|" in l and not l.startswith(("<!--", "#")))
            cm.append(f"  - {fn[:-3]}: {status} ({n} lines)")
    out += ["- codemaps:"] + (cm or ["  - none built yet"])

    # index
    idx = os.path.join(cdir, "index.md")
    if os.path.isfile(idx):
        s = first_line(idx)
        m = re.search(r"systems:(\d+)\s+unmapped:(\d+)\s+unassigned-files:(\d+)", s)
        out.append(f"- index.md: {m.group(1)} systems, {m.group(2)} unmapped, "
                   f"{m.group(3)} files with sys: ?" if m else "- index.md: present")
    else:
        out.append("- index.md: absent — locate.md step 1 has nothing to read")

    # unitymap freshness measured against the files it describes
    um = os.path.join(cdir, "unitymap.md")
    if os.path.isfile(um):
        stamp = first_line(um)
        m = re.search(r"source-sig:(\w+)", stamp)
        try:
            from build_unitymap import source_sig
            from unityparse import walk_assets
            cur = source_sig(sorted(walk_assets(root, (".unity", ".prefab"))))
            fresh = "matches the scenes/prefabs on disk" if m and m.group(1) == cur \
                else "does NOT match the scenes/prefabs on disk (regeneration would change it)"
        except Exception:
            fresh = "freshness not computed"
        extra = re.search(r"status:\s*(.*?)\s*-->", stamp)
        out.append(f"- unitymap.md: {fresh}" + (f"; {extra.group(1)}" if extra else ""))
    else:
        out.append("- unitymap.md: absent — scene/prefab questions would cost a raw YAML read")

    am = os.path.join(cdir, "assetmap.md")
    if os.path.isfile(am):
        s = first_line(am)
        counts = re.findall(r"(assets|prefabs|scenes|asmdefs):(\d+)", s)
        out.append("- assetmap.md: " + (", ".join(f"{n} {k}" for k, n in counts) or "present"))
    else:
        out.append("- assetmap.md: absent — data-source.md has no asset inventory to read")

    # blueprint consistency
    try:
        from check_blueprint import main as bp_main
        import io
        import contextlib
        buf = io.StringIO()
        argv = sys.argv
        sys.argv = ["check_blueprint.py", root]
        with contextlib.redirect_stdout(buf):
            bp_main()
        sys.argv = argv
        tail = [l for l in buf.getvalue().splitlines() if "error(s)" in l]
        errs = [l.replace("[blueprint] ", "") for l in buf.getvalue().splitlines()
                if l.startswith("[blueprint] ERROR")][:3]
        out.append("- blueprint check: " + (tail[0].replace("[blueprint] ", "") if tail else "not run"))
        out += [f"  - {e}" for e in errs]
    except Exception:
        out.append("- blueprint check: not run")

    pre = os.path.join(cdir, "preflight")
    if os.path.isdir(pre):
        cur = os.path.isfile(os.path.join(pre, "current.md"))
        apr = os.path.isfile(os.path.join(pre, "approved"))
        out.append(f"- preflight: current.md {'present' if cur else 'absent'}, "
                   f"approval token {'present' if apr else 'absent'} "
                   f"(approval is session-scoped and is re-checked by the guard)")
    else:
        out.append("- preflight: enforcement directory absent — the guard allows protected writes")

    emit(out)
    return 0


if __name__ == "__main__":
    sys.exit(main())
