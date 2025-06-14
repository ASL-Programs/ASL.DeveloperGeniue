# Contributing Guidelines

Thank you for considering contributing to this project! These guidelines outline the recommended workflow and best practices for contributing code, filing issues, and submitting pull requests.

## Coding Standards

- **Language Style:** Follow generally accepted community standards. For Python code, adhere to [PEP 8](https://peps.python.org/pep-0008/). For JavaScript/TypeScript, follow the [Airbnb style guide](https://github.com/airbnb/javascript) with Prettier for formatting. For C# code, use the .NET conventions enforced by the repository's `.editorconfig` file (four-space indentation, `LF` line endings, braces on new lines).
- **Documentation:** Include docstrings or comments for all major functions, classes, and modules. Keep the README and other documentation up to date with your changes.
- **Commit Messages:** Write clear, concise commit messages. Use the present tense and describe what the commit does (e.g., "Add CONTRIBUTING guidelines").

## Branch Workflow

1. **Fork the Repository:** If you do not have write access, fork the repo to your own GitHub account and clone your fork.
2. **Create a Feature Branch:** Start from `main` and create a branch named after the feature or fix, e.g., `feature/add-api-endpoint` or `bugfix/fix-crash`.
3. **Keep Your Branch Up to Date:** Rebase or merge regularly from `main` to avoid conflicts.
4. **Open a Pull Request:** When your work is ready, push your branch and open a pull request against `main`. Fill out the PR template if provided.

## Testing

- **Add Tests:** Include tests for new features or bug fixes whenever possible. Use the project's existing test framework and aim for high coverage.
- **Run Tests Locally:** Ensure all tests pass locally before opening a pull request. If the project uses continuous integration, your PR should also pass CI checks.

## Code Review Best Practices

- **Small, Focused Changes:** Keep pull requests focused on a single topic or feature to make review easier.
- **Self Review:** Before requesting review, go through your changes to catch typos or obvious issues.
- **Address Feedback Promptly:** Be responsive to reviewer comments and make the requested changes or provide an explanation.

## Submitting Issues

When you encounter a problem or have a feature request:

1. **Search Existing Issues:** Look for existing issues before opening a new one to avoid duplicates.
2. **Provide Details:** Include steps to reproduce the problem, expected behavior, and actual behavior. Attach logs or screenshots if helpful.
3. **Be Respectful:** Follow the project's code of conduct and be respectful in all interactions.

## Submitting Pull Requests

1. **Follow the Branch Workflow:** Make sure your branch is rebased on `main` and tests pass.
2. **Fill Out the PR Description:** Explain what the change does, reference relevant issues, and list any notable implementation details.
3. **Expect Review and Iteration:** Your PR may require revisions before it is merged. Keep discussions constructive and work with reviewers to finalize the change.

We appreciate your contributions and your efforts to keep this project healthy and productive!

