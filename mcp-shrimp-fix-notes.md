# MCP Shrimp Task Manager - DATA_DIR Relative Path Fix

**Date:** January 6, 2026  
**Issue:** Relative `DATA_DIR` paths (like `./.shrimp`) don't resolve to the project directory

---

## Problem Summary

When using `DATA_DIR="./.shrimp"` in the MCP config, the path does NOT resolve relative to your project directory. Instead, it resolves to the mcp-shrimp-task-manager installation directory.

**Expected behavior:** `./.shrimp` → `C:\projects\github\logos-api\.shrimp`  
**Actual behavior:** `./.shrimp` → `C:\mcp-shrimp-task-manager\.shrimp`

---

## Root Cause Analysis

### File: `src/utils/paths.ts` - Function: `getDataDir()`

The code has this priority order for resolving relative paths:

1. **First:** Try `server.listRoots()` to get workspace root from MCP client
2. **If listRoots works:** Use that root + relative DATA_DIR ✅
3. **If listRoots fails/empty:** Fall back to `PROJECT_ROOT` + relative DATA_DIR ❌

### The Problem

- `PROJECT_ROOT` = the mcp-shrimp-task-manager installation directory (e.g., `C:\mcp-shrimp-task-manager`)
- **GitHub Copilot (IntelliJ), Codex, Gemini, and most MCP clients do NOT support `listRoots()`**
- Only Claude Desktop/Claude Code currently support `listRoots()`
- Without `listRoots()`, relative paths resolve to the wrong location

### Current Code (lines 63-77):
```typescript
if (process.env.DATA_DIR) {
  if (path.isAbsolute(process.env.DATA_DIR)) {
    // Absolute path - works fine
    return process.env.DATA_DIR;
  } else {
    // Relative path
    if (rootPath) {
      return path.join(rootPath, process.env.DATA_DIR);
    } else {
      // BUG: Falls back to PROJECT_ROOT (task manager install dir)
      return path.join(PROJECT_ROOT, process.env.DATA_DIR);
    }
  }
}
```

---

## The Fix

Change the fallback from `PROJECT_ROOT` to `process.cwd()` (current working directory).

### Modified Code:
```typescript
if (process.env.DATA_DIR) {
  if (path.isAbsolute(process.env.DATA_DIR)) {
    // If DATA_DIR is an absolute path, use it directly
    return process.env.DATA_DIR;
  } else {
    // If DATA_DIR is a relative path
    if (rootPath) {
      return path.join(rootPath, process.env.DATA_DIR);
    } else {
      // FIX: Use current working directory instead of PROJECT_ROOT
      return path.join(process.cwd(), process.env.DATA_DIR);
    }
  }
}
```

### Also update the comments (optional but nice):
```typescript
// 如果沒有 rootPath，使用當前工作目錄 (process.cwd())
// If there's no rootPath, use current working directory (process.cwd())
return path.join(process.cwd(), process.env.DATA_DIR);
```

---

## Steps to Apply Fix

1. **Open the file:**
   ```
   C:\mcp-shrimp-task-manager\src\utils\paths.ts
   ```

2. **Find this line (around line 75):**
   ```typescript
   return path.join(PROJECT_ROOT, process.env.DATA_DIR);
   ```

3. **Replace with:**
   ```typescript
   return path.join(process.cwd(), process.env.DATA_DIR);
   ```

4. **Rebuild the project:**
   ```bash
   cd C:\mcp-shrimp-task-manager
   npm run build
   ```

5. **Restart your IDE** to reload the MCP server

---

## Will This Work for All MCP Clients?

### Yes, with a caveat:

| MCP Client | listRoots Support | After Fix |
|------------|-------------------|-----------|
| **Claude Desktop** | ✅ Yes | Works (uses listRoots) |
| **Claude Code** | ✅ Yes | Works (uses listRoots) |
| **GitHub Copilot (IntelliJ)** | ❌ No | ✅ Works (uses cwd) |
| **GitHub Copilot (VS Code)** | ❌ No | ✅ Works (uses cwd) |
| **Codex** | ❌ No | ✅ Works (uses cwd) |
| **Gemini** | ❌ No | ✅ Works (uses cwd) |
| **Cline** | ❌ No | ✅ Works (uses cwd) |

### Important Note on `process.cwd()`

The `process.cwd()` returns the **current working directory when the MCP server starts**. This is typically:
- The directory where the IDE was launched from
- Or the project root if the IDE sets cwd properly

**Most IDEs set cwd to the project root**, so `./.shrimp` will resolve to `<project>/.shrimp`.

If an IDE doesn't set cwd to the project, you may still need absolute paths. But this fix makes it work for the vast majority of cases.

---

## Alternative: Use Absolute Paths (No Code Change)

If you don't want to modify the code, use absolute paths in your mcp.json:

```json
{
  "mcp-shrimp-task-manager": {
    "env": {
      "DATA_DIR": "C:\\projects\\github\\logos-api\\.shrimp"
    }
  }
}
```

**Downside:** You need different mcp.json per project, defeating the global config goal.

---

## PR Opportunity

Consider submitting this fix as a PR to the upstream repo:
- **Repo:** https://github.com/cjo4m06/mcp-shrimp-task-manager
- **Issue Title:** "Relative DATA_DIR paths don't work when MCP client doesn't support listRoots"
- **Fix:** Replace `PROJECT_ROOT` with `process.cwd()` in `src/utils/paths.ts`

This would help all users with MCP clients that don't support `listRoots()`.
