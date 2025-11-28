"use client";

import { JSX, useCallback, useEffect, useMemo, useState } from "react";

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

type HighlightKind = "text" | "email" | "iban" | "phone" | "token";
type HighlightDataKind = Exclude<HighlightKind, "text">;

type HighlightSegment =
  | { kind: "text"; value: string }
  | { kind: "email"; value: string }
  | { kind: "iban"; value: string }
  | { kind: "phone"; value: string }
  | { kind: "token"; value: string };

const emailRegex = /[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}/g;
const ibanRegex = /\b[A-Z]{2}[0-9]{13,32}\b/g;
const phoneRegex = /\b\+?[0-9]{7,15}\b/g;
const securityTokenRegex = /\b[A-Za-z0-9_\-]{20,}\b/g;

const apiBaseUrl =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5000";

type DetectedHighlight = {
  kind: HighlightKind;
  startIndex: number;
  length: number;
  value: string;
};

function detectHighlights(text: string): DetectedHighlight[] {
  const highlights: DetectedHighlight[] = [];
  if (text.length === 0) {
    return highlights;
  }

  const collectors: {
    regex: RegExp;
    kind: HighlightKind;
  }[] = [
    { regex: emailRegex, kind: "email" },
    { regex: ibanRegex, kind: "iban" },
    { regex: phoneRegex, kind: "phone" },
    { regex: securityTokenRegex, kind: "token" },
  ];

  collectors.forEach((collector) => {
    const matches = text.matchAll(collector.regex);
    for (const match of matches) {
      if (typeof match.index !== "number") {
        continue;
      }

      const value = match[0] ?? "";
      if (value.length === 0) {
        continue;
      }

      highlights.push({
        kind: collector.kind,
        startIndex: match.index,
        length: value.length,
        value,
      });
    }
  });

  return highlights.sort((left, right) => left.startIndex - right.startIndex);
}

function buildHighlightSegments(text: string): HighlightSegment[] {
  const segments: HighlightSegment[] = [];
  if (text.length === 0) {
    return segments;
  }

  const matches = detectHighlights(text);
  const cursor = { index: 0 };

  matches.forEach((match) => {
    if (match.startIndex < cursor.index) {
      return;
    }

    if (match.startIndex > cursor.index) {
      const plainText = text.slice(cursor.index, match.startIndex);
      if (plainText.length > 0) {
        segments.push({ kind: "text", value: plainText });
      }
    }

    segments.push({ kind: match.kind, value: match.value });
    cursor.index = match.startIndex + match.length;
  });

  if (cursor.index < text.length) {
    const remaining = text.slice(cursor.index);
    if (remaining.length > 0) {
      segments.push({ kind: "text", value: remaining });
    }
  }

  return segments;
}

function renderSegments(segments: HighlightSegment[]): JSX.Element[] {
  const elements: JSX.Element[] = [];

  const styleByKind: Record<HighlightDataKind, string> = {
    email: "underline decoration-wavy decoration-red-500",
    iban: "underline decoration-wavy decoration-emerald-500",
    phone: "underline decoration-wavy decoration-blue-500",
    token: "underline decoration-wavy decoration-yellow-500",
  };

  const titleByKind: Record<HighlightDataKind, string> = {
    email: "PII – Email Address",
    iban: "Finance – IBAN Number",
    phone: "PII – Phone Number",
    token: "Security – Token",
  };

  segments.forEach((segment, segmentIndex) => {
    const keyPrefix = `segment-${segmentIndex}-`;

    if (segment.kind === "text") {
      const parts = segment.value.split("\n");
      parts.forEach((part, index) => {
        if (part.length > 0) {
          elements.push(
            <span key={`${keyPrefix}text-${index}`}>{part}</span>
          );
        }

        if (index < parts.length - 1) {
          elements.push(<br key={`${keyPrefix}br-${index}`} />);
        }
      });
      return;
    }

    const kind: HighlightDataKind = segment.kind;
    const style = styleByKind[kind];
    const title = titleByKind[kind];

    elements.push(
      <span key={`${keyPrefix}${kind}`} className={style} title={title}>
        {segment.value}
      </span>
    );
  });

  return elements;
}

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

  const highlightedSegments = useMemo(() => buildHighlightSegments(text), [text]);
  const highlightedContent = useMemo(
    () => renderSegments(highlightedSegments),
    [highlightedSegments]
  );

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
            <div className="relative h-64 w-full">
              <div
                className="pointer-events-none absolute inset-0 overflow-auto whitespace-pre-wrap rounded-md border border-slate-300 bg-white px-3 py-2 text-sm"
                aria-hidden="true"
              >
                <div className="text-slate-900">{highlightedContent}</div>
              </div>

              <textarea
                className="absolute inset-0 h-full w-full resize-none rounded-md border-none bg-transparent px-3 py-2 text-sm text-slate-900 outline-none focus-visible:ring-2 focus-visible:ring-sky-500"
                value={text}
                onChange={(event) => setText(event.target.value)}
                placeholder="Type or paste text containing email addresses..."
              />
            </div>

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
            <span className="font-semibold">
              {totalSecurityTokens}
            </span>
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
      </main>
    </div>
  );
}
