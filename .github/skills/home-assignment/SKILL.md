---
name: home-assignment
description: "Use when the user receives a timed coding home assignment, take-home task, full-stack exercise, or technical test and needs help understanding requirements, planning, implementing, testing, documenting, recording, or submitting the solution."
---

# Timed Home Assignment Workflow

Use this skill when the user pastes the assignment requirements or says they have received a home assignment. The assignment may be frontend, backend, full-stack, data, automation, or another software task. Do not assume the technology or domain before reading the requirements.

## Operating Principles

- Act as a practical senior coding partner.
- Work directly in the user's open workspace when possible.
- Preserve existing user changes and avoid unrelated refactors.
- Prefer a small, complete, demonstrable solution over many unfinished features.
- Use the project's existing framework, conventions, and package manager when they are already present.
- Ask only questions that block progress. For unclear but non-blocking details, state a reasonable assumption and continue.
- Never expose or commit passwords, API keys, tokens, private connection strings, or personal data.
- Keep the user informed briefly at major milestones.
- Optimize for a finished, explainable submission; protect time for verification and delivery.

## Assignment-Day Control Loop

Before implementation, establish the clock and the submission contract:

1. Record the start time, deadline, expected effort, required deliverables, and the final upload or push method.
2. Reserve the last 15-20% of the available time for testing, README cleanup, recording review, and submission. Do not spend this reserve on new features.
3. Convert every rubric item into an observable acceptance check. Track each item as `not started`, `in progress`, `verified`, or `blocked`, with a file, command, screenshot, or demo step as evidence.
4. Set a scope cutoff. After it, implement only must-haves and fixes to the primary user journey unless a rubric item is still uncovered.
5. At each checkpoint, decide explicitly whether to continue, simplify, or stop. A smaller reliable feature is preferable to a broad feature that cannot be demonstrated.

Suggested checkpoints are:

- 10%: requirements, assumptions, repository, and toolchain confirmed.
- 40%: the main vertical slice works locally.
- 70%: required edge cases and tests are covered.
- 85%: freeze scope and prepare the submission.
- 100%: verify the artifact and submit.

If the user gives a rubric, preserve its wording in the acceptance checklist. If there is no rubric, create checks for functionality, quality, tests, documentation, UX, and delivery.

## Preflight and Recovery

Run a short preflight before making product changes:

- Confirm the required runtime, package manager, database, services, ports, and account access.
- Install dependencies and run the existing tests or build once, recording any baseline failure.
- Confirm how the evaluator will start the project and whether network access, seeded data, or environment variables are required.
- Check whether recording, repository visibility, branch naming, commit history, or artifact size rules apply.

When a dependency or service is unavailable:

1. Capture the exact failure and distinguish an environment problem from a code problem.
2. Continue with a deterministic local substitute, fixture, mock, or in-memory implementation only when it preserves the required contract.
3. Add a clear setup note and limitation to `README.md`.
4. Re-run every check that remains meaningful and label unavailable checks honestly.

Never hide a failing baseline or silently change the required behavior to make a check pass.

## Phase 1: Read and Convert Requirements

When the user provides the assignment:

1. Read the complete text before editing.
2. Extract the following:
   - Required functionality.
   - Users, roles, and permissions.
   - Inputs, outputs, and expected user flows.
   - Required technologies, versions, database, and deployment constraints.
   - Required tests, documentation, screen recording, and Git submission rules.
   - Deadline and any time limit.
3. Separate requirements into:
   - Must have.
   - Should have.
   - Nice to have.
   - Open questions or assumptions.
4. Give the user a concise summary and state the first falsifiable implementation hypothesis.
5. Identify the cheapest check that can disconfirm the hypothesis.

If the requirements are incomplete, do not invent complex features. Choose the simplest behavior that satisfies the written request and record the assumption in `README.md`.

## Phase 2: Inspect and Plan

Before the first edit, inspect only the relevant local project surface:

- Existing files and project structure.
- Package manifests and configuration.
- Nearby implementations and tests.
- Available build and test commands.

Then create a short implementation plan with ordered steps. The plan should normally cover:

1. Project setup or confirmation.
2. Data model and API or component boundaries.
3. The main end-to-end user flow.
4. Validation and error states.
5. Tests or manual verification.
6. README and submission preparation.

Use a task checklist when the work has multiple steps. Keep exactly one active step at a time and update it as work progresses.

## Phase 3: Implement the Smallest Vertical Slice

Build one complete path first, from user action to persisted or returned result. Examples:

- Form -> API -> database -> response -> UI update.
- Input -> processing -> output -> error handling.
- Command -> business logic -> test -> documented usage.

Follow these rules:

- Reuse existing abstractions before adding new ones.
- Keep public APIs and naming consistent with the project.
- Add only the dependencies that are necessary.
- Validate external input at the boundary.
- Handle expected failures with useful responses or messages.
- Avoid premature architecture and unrelated cleanup.
- Add concise comments only for non-obvious decisions.

After the first substantive edit, immediately run the narrowest available validation before reading or changing unrelated areas. If it fails, repair the same slice and rerun that check.

## Phase 4: Validate Deliberately

Validation order:

1. The cheapest behavior-focused check for the changed path.
2. A focused unit, integration, or component test.
3. A focused build, typecheck, or lint command.
4. A full build or test suite when practical.
5. A manual smoke test of the main user journey.

Check at least:

- Happy path.
- Empty or invalid input.
- Not-found or unauthorized behavior where relevant.
- Duplicate or repeated actions where relevant.
- Loading, error, and success states in the UI where relevant.
- Configuration with missing environment variables.

Report validation honestly. Distinguish passed checks from checks that could not be run because a dependency, service, or credential is unavailable.

Keep an acceptance matrix while validating:

| Requirement | Verification | Evidence | Status |
| --- | --- | --- | --- |
| Must-have behavior | Test or deterministic manual step | Test name, command, or screenshot | `not started` |

For each failed check, record the symptom, likely cause, impact, and next action. Re-test the original failure after fixing it. Do not treat a successful compile as proof that the user journey works.

### Tests as Quality Evidence

Save automated tests in the repository so the evaluator can run them, review them, and see the intended behavior. Do not rely only on screenshots, console output, or a claim that the feature was tested.

- Follow the test location and naming conventions already used by the project. If none exist, use the framework's standard convention and document it in `README.md`.
- Organize tests around observable behavior and requirements, not private implementation details.
- Cover the highest-risk paths first: the primary happy path, invalid or empty input, authorization or not-found behavior where relevant, and duplicate or repeated actions where relevant.
- Keep test data deterministic. Use fixtures, factories, or isolated setup so tests do not depend on a personal database, machine state, test order, or live third-party service.
- Keep each test focused on one behavior, with a clear setup, action, and expected result. Test names should explain the behavior that must remain true.
- Include at least one end-to-end or integration test for the main vertical slice when the assignment has multiple layers such as UI, API, and database.
- For frontend work, test user-visible states and interactions rather than only component rendering: loading, success, empty, validation, and error states where they apply.
- Run the relevant test command after each substantial change, then run the complete test suite before submission.
- Record the exact command and result in the acceptance matrix and `README.md`. Keep the tests and the commands needed to run them in the final submission.

Strong evidence is a small, readable test suite that maps directly to the rubric. More tests are not automatically better if they are brittle, duplicated, or disconnected from user behavior.

## Phase 5: Documentation and Git Submission

Ensure the project has a useful `README.md` containing:

- What the solution does.
- Main technical decisions.
- Prerequisites and versions.
- Environment variables, using placeholders only.
- Installation and run commands.
- Test commands and manual test steps.
- Assumptions and known limitations.
- General `AI Usage` disclosure if AI was used.

Before submission, inspect:

```powershell
git status
git diff --check
git log --oneline -5
```

Verify that:

- Required files are tracked.
- Secrets and generated dependencies are ignored.
- The README instructions work from a clean checkout when possible.
- The recording file exists, opens, and is readable.
- The repository permissions and link match the assignment rules.
- The final push completed before the stated deadline.

Do not commit or push unless the user explicitly asks for it or the assignment workflow clearly requires it and the user has authorized that action.

### Clean-Checkout Test

When time permits, validate from a fresh checkout or a clean temporary directory. Follow the README exactly, including dependency installation, environment setup, database initialization, and test commands. This catches undocumented local state, missing files, incorrect paths, and accidental reliance on ignored artifacts.

## Professional Presentation

Treat the implementation and the delivery as one professional artifact. Optimize for clarity, reliability, and the ability to explain decisions.

### Code and Architecture

- Use names that communicate intent and keep responsibilities clear.
- Keep controllers, services, data access, and UI components focused on one concern where the chosen stack supports that separation.
- Prefer simple, explicit code over clever abstractions.
- Keep configuration in environment variables or configuration files, never hardcoded secrets.
- Remove template endpoints, unused imports, dead code, debug output, and accidental generated files before submission.
- Make validation, authorization, and error handling visible at the correct boundary.

### User Experience

- Make the main flow easy to discover and complete.
- Provide clear loading, success, empty, and error states where relevant.
- Use consistent spacing, typography, labels, and button behavior.
- Check the main screen at both desktop and mobile widths when a frontend is included.
- Avoid adding visual decoration that does not improve the task or explain the product.

### Explanation and Demo

Be ready to explain in plain language:

- What problem the solution addresses.
- The main user flow.
- The most important design or technical decision.
- One tradeoff or limitation and what could be improved next.
- How the solution was tested.

When presenting the work, demonstrate the main successful flow first, then one validation or error case. Keep the demo short, deterministic, and based on prepared test data. Do not improvise new features during the final recording.

### Final Professional Review

Before submission, check:

- The repository opens cleanly and has a clear structure.
- The project name and README look intentional.
- The first command a reviewer needs is easy to find.
- API responses, UI messages, and validation text are understandable.
- Commit messages describe meaningful milestones.
- The final recording shows the work clearly without secrets or distracting personal information.
- You can explain the solution without saying that AI made the decisions for you.

Run a final reviewer simulation:

1. Start from the documented command and use the prepared data or credentials described in the README.
2. Demonstrate each must-have requirement once, in rubric order, without navigating through unfinished features.
3. Demonstrate one useful validation or failure state and explain the expected behavior.
4. Inspect the diff for accidental files, debug output, secrets, generated artifacts, and changes outside scope.
5. Read the README as an evaluator: it should answer how to run, test, evaluate, and understand the tradeoffs without needing the author present.

## Screen Recording Awareness

If screen recording is required:

- Remind the user to start recording before the relevant work begins.
- Keep terminal output and editor content readable.
- Avoid displaying secrets in the recording.
- At the end, check that the recording file is present and mention its expected Git location.

## AI Usage

Use AI for decomposition, explanations, boilerplate, debugging, test ideas, and review. Never claim that generated code was tested unless it was actually tested. When a generated solution is changed, explain the important decision briefly so the user can understand and present it.

## Response Pattern on Assignment Day

Start with:

1. A concise interpretation of the task.
2. Must-have requirements and assumptions.
3. The immediate plan.
4. Any single blocking question, if one exists.

Then act. Do not stop at a proposal when implementation is possible. At the end, provide:

- What was implemented.
- Which files changed.
- Validation performed and results.
- Remaining limitations.
- Exact run and submission steps.

## User Prompt Template

The user can start the workflow with:

> יש לי מטלת בית. הנה כל הדרישות: [להדביק כאן את הטקסט המלא]. הדדליין הוא: [שעה ותאריך]. צריך גם: [בדיקות / הקלטת מסך / Git / README].

If the user does not provide a deadline, ask for it only if timing affects the implementation plan; otherwise start by extracting the requirements.
