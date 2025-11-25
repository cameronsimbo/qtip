"use client";

import { JSX, useCallback, useEffect, useMemo, useState } from "react";

type SubmitResponse = {
  tokenizedText: string;
  detectedEmailCount: number;
};

type StatsResponse = {
  totalPiiEmails: number;
};

type HighlightSegment =
  | { kind: "text"; value: string }
  | { kind: "email"; value: string };

const emailRegex = /[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}/g;

const apiBaseUrl =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5000";

function buildHighlightSegments(text: string): HighlightSegment[] {
  const segments: HighlightSegment[] = [];
  if (text.length === 0) {
    return segments;
  }

  let currentIndex = 0;
  const matches = text.matchAll(emailRegex);

  for (const match of matches) {
    if (typeof match.index !== "number") {
      continue;
    }

    const startIndex = match.index;
    const value = match[0] ?? "";

    if (startIndex > currentIndex) {
      const plainText = text.slice(currentIndex, startIndex);
      if (plainText.length > 0) {
        segments.push({ kind: "text", value: plainText });
      }
    }

    if (value.length > 0) {
      segments.push({ kind: "email", value });
    }

    currentIndex = startIndex + value.length;
  }

  if (currentIndex < text.length) {
    const remaining = text.slice(currentIndex);
    if (remaining.length > 0) {
      segments.push({ kind: "text", value: remaining });
    }
  }

  return segments;
}

function renderSegments(segments: HighlightSegment[]): JSX.Element[] {
  const elements: JSX.Element[] = [];

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
    } else {
      elements.push(
        <span
          key={`${keyPrefix}email`}
          className="underline decoration-wavy decoration-red-500"
          title="PII – Email Address"
        >
          {segment.value}
        </span>
      );
    }
  });

  return elements;
}

export default function Home(): JSX.Element {
  const [text, setText] = useState<string>("");
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [totalPiiEmails, setTotalPiiEmails] = useState<number>(0);

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

        if (typeof data.detectedEmailCount === "number") {
          // We keep the original text in the input; tokenized text is not shown
          // because the challenge does not require it in the UI.
        }

        await fetchStats();
      } catch {
        setSubmitError("An unexpected error occurred while submitting.");
      } finally {
        setIsSubmitting(false);
      }
    },
    [fetchStats, text]
  );

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      <main className="mx-auto flex min-h-screen max-w-4xl flex-col gap-8 px-4 py-10">
        <section>
          <h1 className="text-2xl font-semibold tracking-tight">
            QTip – PII Email Detector
          </h1>
          <p className="mt-2 text-sm text-slate-600">
            Paste or type text below. Email addresses will be underlined and
            marked as PII in real time.
          </p>
        </section>

        <section>
          <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
            <div className="relative">
              <div
                className="pointer-events-none absolute inset-0 whitespace-pre-wrap rounded-md border border-slate-300 bg-white px-3 py-2 text-sm text-transparent"
                aria-hidden="true"
              >
                <div className="text-slate-900">{highlightedContent}</div>
              </div>

              <textarea
                className="relative h-64 w-full resize-none rounded-md border border-slate-300 bg-transparent px-3 py-2 text-sm text-slate-900 outline-none focus-visible:ring-2 focus-visible:ring-sky-500"
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

              {submitError !== null ? (
                <p className="text-sm text-red-600">{submitError}</p>
              ) : null}
            </div>
          </form>
        </section>

        <section className="rounded-md border border-slate-200 bg-white px-4 py-3 text-sm text-slate-800">
          <p>
            Total PII emails submitted:{" "}
            <span className="font-semibold">{totalPiiEmails}</span>
          </p>
        </section>
      </main>
    </div>
  );
}
