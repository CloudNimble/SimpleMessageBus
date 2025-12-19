## General

* Make only high confidence suggestions when reviewing code changes.
* Always use the latest version C#. When global.json targets .NET 8, use C# 12. When it targets .NET 9.0, use C# 13. When it uses .NET 10.0, target C# 14.
* Never change global.json unless explicitly asked to.
* Always prefer `.IsNullOrWhiteSpace()` over `.IsNullOrEmpty()` for strings.
* Always prefer `ArgumentException.ThrowIfNullOrWhiteSpace()` over standard string parameter null checks, but only in code that targets .NET 8 or later.
* Always prefer `ArgumentNullException.ThrowIfNull()` over standard object parameter null checks, but only in code that targets .NET 8 or later.
* Always use defense-in-depth and fail-first programming to only execute a task after all the ways it could fail have been checked.
* Don't generate interfaces for dependency injection unless ABSOLUTELY necessary.

## Formatting

* Apply code-formatting style defined in `.editorconfig`.
* Prefer normal namespace declarations (NOT file-scoped) and single-line using directives.
* Insert a newline before the opening curly brace of any code block (e.g., after `if`, `for`, `while`, `foreach`, `using`, `try`, etc.).
* Ensure that the final return statement of a method is on its own line.
* Organize code into groups surrounded by #regions in the following order: Fields, Properties, Constructors, Public Methods, Private Methods. 
  - Region instructions should be surrounded by blank lines.
* Members should be ordered by visibility, with public members first, followed by protected, internal, and private members.
* Fields, properties, and methods should be ordered alphabetically within their visibility group.
* Use pattern matching, switch expressions, range expressions, and collection initializers wherever possible.
* Use `nameof` instead of string literals when referring to member names.
* Ensure that extensive XML doc comments are created for any APIs. When applicable, include <example> and <code> documentation in the comments. Only <param> tags should be on the same line as content.

### Nullable Reference Types

* Declare variables non-nullable, and check for `null` at entry points.
* Always use `is null` or `is not null` instead of `== null` or `!= null`.
* Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.

### Testing

* We use MSTest v3, Breakdance, and FluentAssertions for tests.
* Do not emit "Act", "Arrange" or "Assert" comments.
* Do not use any mocking in tests.
* Copy existing style in nearby files for test method names and capitalization.
* Always prefer `.NotBeNullOrWhiteSpace()` over `.NotBeNullOrEmpty()` for testing strings.