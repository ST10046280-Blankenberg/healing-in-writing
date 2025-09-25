# Security Notes

<p align="center">
  <img src="https://img.shields.io/badge/Secrets-Keep%20Out-critical?style=flat-square" alt="No secrets">
  <img src="https://img.shields.io/badge/Config-appsettings.Development.json-blue?style=flat-square" alt="Use dev config">
  <img src="https://img.shields.io/badge/Incidents-Report%20Immediately-orange?style=flat-square" alt="Report incidents">
</p>

Keep the codebase free of secrets and handle sensitive data carefully.

---

## Handling Sensitive Data
- Never commit secrets, keys, or personal data into the repository.
- Store local configuration values in `appsettings.Development.json` and user secrets.

## Incident Response
1. Pause any automated deployments touching the affected data.
2. Notify the PM immediately with a concise summary of the issue.
3. Open a private ticket containing timelines and remediation steps.
4. Coordinate a fix, documenting any configuration or data clean-up carried out.
5. Record lessons learned so we avoid similar issues next time.

> If you are unsure whether data is sensitive, assume it is and escalate.
