# Task Completion Checklist

When you complete a coding task in the Chaos-Server project, ensure you perform the following steps:

## 1. Code Quality Checks

### Verify Code Compiles
```powershell
dotnet build Chaos.sln
```
- Ensure no build errors or warnings
- Check that all projects build successfully

### Run Tests
```powershell
# Run all tests to ensure nothing is broken
dotnet test Chaos.sln --nologo

# If you modified specific areas, run focused tests
dotnet test Chaos.sln --filter "FullyQualifiedName~AffectedArea" --nologo
```

### Check for Linting/Formatting Issues
- Ensure code follows the project's coding conventions
- Check indentation matches surrounding code (tabs vs spaces)
- Verify naming conventions are followed

## 2. Code Review Checklist

Before considering a task complete, verify:

- [ ] **No hardcoded values** - Use configuration or constants
- [ ] **No exposed secrets** - Never log or expose sensitive data
- [ ] **Error handling** - Appropriate try-catch blocks where needed
- [ ] **Edge cases handled** - Consider null checks, boundary conditions
- [ ] **Existing patterns followed** - Code matches project conventions
- [ ] **No unnecessary files created** - Prefer editing existing files
- [ ] **XML documentation added** - For new public APIs
- [ ] **Tests updated/added** - If modifying testable logic

## 3. Template and Script Validation

If you modified templates or scripts:
- [ ] JSON templates are valid and follow schema
- [ ] Script keys match class names (without "Script" suffix)
- [ ] ScriptVars are properly configured
- [ ] No breaking changes to existing content

## 4. Configuration Updates

If configuration changes were made:
- [ ] Updated appropriate appsettings file
- [ ] Documented new configuration options
- [ ] Ensured backward compatibility
- [ ] Updated appsettings.local.json template if needed

## 5. Performance Considerations

For performance-critical changes:
- [ ] No unnecessary allocations in hot paths
- [ ] Efficient collection usage
- [ ] Appropriate caching where beneficial
- [ ] No blocking operations in async contexts

## 6. Security Review

Before finalizing:
- [ ] No SQL injection vulnerabilities
- [ ] Input validation implemented
- [ ] No sensitive data in logs
- [ ] Proper authentication/authorization checks
- [ ] No path traversal vulnerabilities

## 7. Documentation

If applicable:
- [ ] Update relevant XML documentation comments
- [ ] Update CLAUDE.md if adding new workflows
- [ ] Document breaking changes
- [ ] Update configuration documentation

## 8. Final Verification

```powershell
# Final build to ensure everything compiles
dotnet build Chaos.sln

# Run full test suite one more time
dotnet test Chaos.sln --nologo
```

## 9. Git Considerations

When committing (only if explicitly requested):
- Use descriptive commit messages
- Follow repository's commit message format
- Never commit:
  - appsettings.local.json
  - Secrets or API keys
  - Debug/bin directories
  - User-specific files

## Important Reminders

- **NEVER proactively create documentation files** unless explicitly requested
- **Always prefer editing existing files** over creating new ones
- **Follow existing code patterns** in the codebase
- **Test your changes** before marking task complete
- **Ask for clarification** if requirements are unclear

If any of these checks fail, address the issues before considering the task complete.