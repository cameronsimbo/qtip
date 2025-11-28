"use client";

import { JSX, useCallback, useEffect, useState } from "react";
import { HighlightingTextArea } from "./components/HighlightingTextArea";
import { StatsPanel } from "./components/StatsPanel";

type SubmitResponse = {
  tokenizedText: string;
  detectedPiiEmails: number;
  detectedFinanceIbans: number;
  detectedPiiPhones: number;
  detectedSecurityTokens: number;
};

type StatsResponse = {
  totalPiiEmails: number;
  totalFinanceIbans: number;
  totalPiiPhones: number;
  totalSecurityTokens: number;
  lastSubmissionPiiEmails: number;
  lastSubmissionFinanceIbans: number;
  lastSubmissionPiiPhones: number;
  lastSubmissionSecurityTokens: number;
};

const apiBaseUrl =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5000";

export default function Home(): JSX.Element {
  const [text, setText] = useState<string>("");
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [totalPiiEmails, setTotalPiiEmails] = useState<number>(0);
  const [totalFinanceIbans, setTotalFinanceIbans] = useState<number>(0);
  const [totalPiiPhones, setTotalPiiPhones] = useState<number>(0);
  const [totalSecurityTokens, setTotalSecurityTokens] = useState<number>(0);
  const [lastSubmissionPiiEmails, setLastSubmissionPiiEmails] = useState<number>(0);
  const [lastSubmissionFinanceIbans, setLastSubmissionFinanceIbans] = useState<number>(0);
  const [lastSubmissionPiiPhones, setLastSubmissionPiiPhones] = useState<number>(0);
  const [lastSubmissionSecurityTokens, setLastSubmissionSecurityTokens] = useState<number>(0);

  const fetchStats = useCallback(async () => {
    try {
      const response = await fetch(`${apiBaseUrl}/api/stats/emails`, {
        method: "GET",
      });

      if (!response.ok) {
        return;
      }

      const data: StatsResponse = await response.json();
      setTotalPiiEmails(data.totalPiiEmails);
      setTotalFinanceIbans(data.totalFinanceIbans);
      setTotalPiiPhones(data.totalPiiPhones);
      setTotalSecurityTokens(data.totalSecurityTokens);
      setLastSubmissionPiiEmails(data.lastSubmissionPiiEmails);
      setLastSubmissionFinanceIbans(data.lastSubmissionFinanceIbans);
      setLastSubmissionPiiPhones(data.lastSubmissionPiiPhones);
      setLastSubmissionSecurityTokens(data.lastSubmissionSecurityTokens);
    } catch {
      // For this challenge, we silently ignore stats errors.
    }
  }, []);

  useEffect(() => {
    void fetchStats();
  }, [fetchStats]);

  const handleSubmit = useCallback(
    async (event: React.FormEvent<HTMLFormElement>) => {
      event.preventDefault();

      setIsSubmitting(true);
      setSubmitError(null);

      try {
        const response = await fetch(`${apiBaseUrl}/api/submissions`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ text }),
        });

        if (!response.ok) {
          const errorText = `Submission failed with status ${response.status}`;
          setSubmitError(errorText);
          return;
        }

        const data: SubmitResponse = await response.json();

        await fetchStats();
      } catch {
        setSubmitError("An unexpected error occurred while submitting.");
      } finally {
        setIsSubmitting(false);
      }
    },
    [fetchStats, text]
  );

  const handleClear = useCallback(async () => {
    setSubmitError(null);

    try {
      const response = await fetch(`${apiBaseUrl}/api/stats/clear`, {
        method: "POST",
      });

      if (!response.ok) {
        const errorText = `Clear failed with status ${response.status}`;
        setSubmitError(errorText);
        return;
      }

      await fetchStats();
    } catch {
      setSubmitError("An unexpected error occurred while clearing stats.");
    }
  }, [fetchStats]);

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      <main className="mx-auto flex min-h-screen max-w-4xl flex-col gap-8 px-4 py-10">
        <section>
          <h1 className="text-2xl font-semibold tracking-tight">
            QTip – PII and Sensitive Data Detector
          </h1>
          <p className="mt-2 text-sm text-slate-600">
            Paste or type text below. Detected emails, IBANs, phone numbers,
            and security tokens will be underlined and marked in real time.
          </p>
        </section>

        <section>
          <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
            <HighlightingTextArea
              value={text}
              onChange={setText}
              placeholder="Type or paste text containing email addresses..."
            />

            <div className="flex items-center gap-4">
              <button
                type="submit"
                className="inline-flex items-center justify-center rounded-md bg-sky-600 px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-sky-700 disabled:cursor-not-allowed disabled:bg-sky-300"
                disabled={isSubmitting || text.trim().length === 0}
              >
                {isSubmitting ? "Submitting..." : "Submit Text"}
              </button>

              <button
                type="button"
                className="inline-flex items-center justify-center rounded-md border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 transition-colors hover:bg-slate-100"
                onClick={() => {
                  void handleClear();
                }}
              >
                Clear stats
              </button>

              {submitError !== null ? (
                <p className="text-sm text-red-600">{submitError}</p>
              ) : null}
            </div>
          </form>
        </section>

        <StatsPanel
          totalPiiEmails={totalPiiEmails}
          totalFinanceIbans={totalFinanceIbans}
          totalPiiPhones={totalPiiPhones}
          totalSecurityTokens={totalSecurityTokens}
          lastSubmissionPiiEmails={lastSubmissionPiiEmails}
          lastSubmissionFinanceIbans={lastSubmissionFinanceIbans}
          lastSubmissionPiiPhones={lastSubmissionPiiPhones}
          lastSubmissionSecurityTokens={lastSubmissionSecurityTokens}
        />
      </main>
    </div>
  );
}
