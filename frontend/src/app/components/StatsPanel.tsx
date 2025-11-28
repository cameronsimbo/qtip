import { JSX } from "react";

type StatsPanelProps = {
  totalPiiEmails: number;
  totalFinanceIbans: number;
  totalPiiPhones: number;
  totalSecurityTokens: number;
  lastSubmissionPiiEmails: number;
  lastSubmissionFinanceIbans: number;
  lastSubmissionPiiPhones: number;
  lastSubmissionSecurityTokens: number;
};

export function StatsPanel(props: StatsPanelProps): JSX.Element {
  const {
    totalPiiEmails,
    totalFinanceIbans,
    totalPiiPhones,
    totalSecurityTokens,
    lastSubmissionPiiEmails,
    lastSubmissionFinanceIbans,
    lastSubmissionPiiPhones,
    lastSubmissionSecurityTokens,
  } = props;

  return (
    <section className="rounded-md border border-slate-200 bg-white px-4 py-3 text-sm text-slate-800">
      <h2 className="text-sm font-semibold text-slate-900">
        Totals (all submissions)
      </h2>
      <p className="mt-1">
        Total PII emails submitted:{" "}
        <span className="font-semibold">{totalPiiEmails}</span>
      </p>
      <p className="mt-1">
        Total finance IBANs detected:{" "}
        <span className="font-semibold">{totalFinanceIbans}</span>
      </p>
      <p className="mt-1">
        Total PII phone numbers detected:{" "}
        <span className="font-semibold">{totalPiiPhones}</span>
      </p>
      <p className="mt-1">
        Total security tokens detected:{" "}
        <span className="font-semibold">{totalSecurityTokens}</span>
      </p>

      <h2 className="mt-4 text-sm font-semibold text-slate-900">
        Most recent submission
      </h2>
      <p className="mt-1">
        PII emails:{" "}
        <span className="font-semibold">
          {lastSubmissionPiiEmails}
        </span>
      </p>
      <p className="mt-1">
        Finance IBANs:{" "}
        <span className="font-semibold">
          {lastSubmissionFinanceIbans}
        </span>
      </p>
      <p className="mt-1">
        PII phone numbers:{" "}
        <span className="font-semibold">
          {lastSubmissionPiiPhones}
        </span>
      </p>
      <p className="mt-1">
        Security tokens:{" "}
        <span className="font-semibold">
          {lastSubmissionSecurityTokens}
        </span>
      </p>
    </section>
  );
}


