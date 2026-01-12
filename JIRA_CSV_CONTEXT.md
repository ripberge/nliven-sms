# Jira CSV Import Context

This document preserves the context for generating and updating the Jira CSV file for the SMS Notification Service implementation plan.

## Jira Instance Details

- **Instance:** https://tixtrack.atlassian.net
- **Project Key:** `NLVN` (Nliven Development)
- **Epic Key:** `NLVN-28094` (SMS Ticket Delivery)
- **Epic Status:** To Do
- **Priority:** P2 - High

## Custom Field IDs (from XML export)

These are the custom field IDs used by the NLVN project. Use these when mapping CSV columns during import or API calls:

| Custom Field Name | Field ID | Type | Usage |
|---|---|---|---|
| Sprint | `customfield_10016` | Sprint Field | Maps stories to sprints (Sprint 1, Sprint 2, etc.) |
| Rank | `customfield_10015` | Rank (Lexo) | Internal ranking field |
| Nliven Team | `customfield_11697` | Radio Button | Examples: "Buying", "Shipping", "Development" |
| Nliven Workstream | `customfield_12163` | Dropdown | Examples: "Ticket Delivery", "API Platform" |
| Product Manager | `customfield_11535` | User Picker | Examples: "Diego Miranda" |
| Client: Project | `customfield_11468` | Multi-select | Examples: "TixTrack: General" |
| Approvals | `customfield_10200` | Service Desk | Approval tracking |
| Development | `customfield_10400` | Dev Summary | GitHub integration |
| Epic Color | `customfield_10020` | Epic Color | Kanban board display |
| Epic Status | `customfield_10018` | Epic Status | "To Do", "In Progress", "Done" |
| Issue Color | `customfield_11440` | Issue Color | Board display (green, blue, yellow, etc.) |
| Organizations | `customfield_10600` | Service Desk | Customer organizations |

## Standard Field Values

### Priority
- `Highest`
- `High`
- `Medium`
- `Low`

### Issue Type
- `Story` (for work items)
- `Task` (for infrastructure/support work)
- `Epic` (for grouping stories)

## CSV Generation Instructions

### When to Regenerate CSV
1. **After modifying story details** in the implementation plan (descriptions, acceptance criteria, story points)
2. **When adding new sprints or stories** to the plan
3. **When updating technical notes** that should be reflected in Jira descriptions

### Regeneration Steps
1. Extract story information from `implementation-plan-jira-tickets.md`
2. Build CSV with these columns (minimum):
   - `Summary` - Story title (short form, e.g., "1.1 - Create Domain Models")
   - `Description` - Full description with acceptance criteria in paragraph form
   - `Issue Type` - "Story" (or "Task" for infrastructure items)
   - `Priority` - "Highest", "High", "Medium", or "Low"
   - `Story Points` - Number of points
   - `Sprint` - Sprint numeric ID (1138 for AUS on CCV2)
   - `Parent` - Always `NLVN-28094` for this epic (Jira Cloud uses "Parent" field)
   - `Assignee` - Leave blank or assign to specific person

3. Use Jira's **Import Issues** feature:
   - Project → Tools → Import Issues (or Project Settings → Import/Export)
   - Select CSV file
   - Map columns to Jira fields during import
   - For custom fields, match by field ID or name
   - Click Import

### Optional CSV Columns
To avoid duplicate creation and control assignment/workflow:
- Add `Updated` date to prevent re-importing already-created issues
- Add `Assignee` column to auto-assign stories
- Add custom field columns for Nliven Team, Workstream, Product Manager
- Note: Use `Parent` (not `Epic Link`) for Jira Cloud to link to epic NLVN-28094

### Testing & Documentation
All stories include acceptance criteria for:
- **Unit Tests:** >80% code coverage with xUnit, Moq for mocking
- **Integration Tests:** Where applicable (Service Bus, webhooks, database)
- **Documentation:** Code comments, technical notes, operational runbooks (in deployment stories)
- **No Separate Sprint:** Testing and documentation are part of each story's definition of done

## Sample CSV Header Row

```csv
Summary,Description,Issue Type,Priority,Story Points,Sprint,Assignee,Parent
```

## Sample CSV Data Row

```csv
"1.1 - Create Domain Models","Create the core domain models (SmsMessage, VenuePhoneNumber, SmsSendHistory) aligned with nLiven entity patterns.

Acceptance Criteria:
- Implement SmsMessage domain object with status tracking
- Implement VenuePhoneNumber with FK to nLiven Venue.ID
- Implement SmsSendHistory with timestamp and provider tracking
- All models use int ID fields per nLiven pattern
- Enum status fields map to lookup tables",Story,Medium,3,1138,,NLVN-28094
```

## Epic Reference

**Title:** SMS Ticket Delivery
**Key:** NLVN-28094
**Status:** To Do
**Assignee:** Diego Miranda
**Product Manager:** Diego Miranda
**Nliven Team:** Buying
**Nliven Workstream:** Ticket Delivery
**Client: Project:** TixTrack: General

All stories created for this epic should have `NLVN-28094` in the "Epic Link" field to automatically associate them with this epic.

## How to Update Implementation Plan and Regenerate CSV

1. **Modify** `implementation-plan-jira-tickets.md` with new stories or updated details
2. **Generate new CSV** by extracting stories following the format above
3. **Create new Jira issues** via CSV import (Jira will detect duplicates based on Summary and not re-create)
4. **Update existing issues** by modifying the CSV and re-importing (use `Updated` field to track changes)

## Version History

- **Created:** January 7, 2026
- **Initial Stories:** 37 stories across 11 sprints
- **Updated:** January 12, 2026
- **Current Stories:** 43 stories across 9 sprints (consolidated testing/documentation into each story, restored foundation stories)
- **Project Key:** NLVN
- **Epic:** NLVN-28094 (SMS Ticket Delivery)

### Sprint Breakdown & Parallelization

**All Stories - AUS on CCV2 Sprint (ID: 1138)**

All 43 stories are assigned to Sprint ID 1138 (AUS on CCV2) and organized into 9 logical work phases:

**Phase 1 - Skeleton Service & Infrastructure (7 stories, 32 pts)**
- 1.1: Project Setup (blocker, 3 pts)
- 1.2.1, 1.2.2: Minimal API + MVP Plivo (parallel after 1.1, 5+8 pts)
- 1.3.1, 1.3.2: Domain Models + API Endpoint Design (parallel after 1.1, 3+3 pts)
- 1.4.1, 1.4.2, 1.4.3: Database + Repositories + Service Bus (1.4.1 after 1.3.1, then 1.4.2 and 1.4.3 parallel, 5+5+5 pts)

**Phase 2 - Infrastructure as Code (3 stories, 21 pts)**
- 2.1: GitHub Actions CI/CD (parallel after 1.1, 5 pts)
- 2.2, 2.3: Terraform Base + SQL/Service Bus (parallel, 8+8 pts)

**Phase 3 - Core Services (4 stories, 16 pts)**
- 3.1-3.4: Logging, Consent, Phone Number, Rate Limiting

**Phase 4 - SMS Providers (2 stories, 10 pts)**
- 4.1-4.2: Twilio & Plivo Adapters

**Phase 5 - Webhooks (2 stories, 8 pts)**
- 5.1-5.2: Delivery Receipts & Signature Verification

**Phase 6 - Event-Driven (3 stories, 13 pts)**
- 6.1-6.3: Service Bus Consumer, Events, Dead-Letter Queue

**Phase 7 - Advanced Features (2 stories, 8 pts)**
- 7.1-7.2: Inbound SMS, Status Webhooks

**Phase 8 - Resilience & Performance (3 stories, 11 pts)**
- 8.1-8.3: Failover, Load Balancing, Metrics

**Phase 9 - Production Readiness (3 stories, 18 pts)**
- 9.1-9.3: Load Testing, Security Audit, Deployment & Go-Live
