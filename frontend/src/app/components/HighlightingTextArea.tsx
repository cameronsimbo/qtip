import { JSX, useMemo, useRef } from "react";

type HighlightKind = "text" | "email" | "iban" | "phone" | "token";
type HighlightDataKind = Exclude<HighlightKind, "text">;

type HighlightSegment =
  | { kind: "text"; value: string }
  | { kind: "email"; value: string }
  | { kind: "iban"; value: string }
  | { kind: "phone"; value: string }
  | { kind: "token"; value: string };

type DetectedHighlight = {
  kind: HighlightKind;
  startIndex: number;
  length: number;
  value: string;
};

const emailRegex = /[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}/g;
const ibanRegex = /\b[A-Z]{2}[0-9]{13,32}\b/g;
const phoneRegex = /\b\+?[0-9]{7,15}\b/g;
const securityTokenRegex = /\b[A-Za-z0-9_\-]{20,}\b/g;

type HighlightingTextAreaProps = {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
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
      <span
        key={`${keyPrefix}${kind}`}
        className={`${style} pointer-events-auto`}
        title={title}
      >
        {segment.value}
      </span>
    );
  });

  return elements;
}

export function HighlightingTextArea(
  props: HighlightingTextAreaProps,
): JSX.Element {
  const { value, onChange, placeholder } = props;
  const textareaRef = useRef<HTMLTextAreaElement | null>(null);
  const overlayRef = useRef<HTMLDivElement | null>(null);
  const isSyncingScrollRef = useRef<boolean>(false);

  const segments = useMemo(() => buildHighlightSegments(value), [value]);
  const highlightedContent = useMemo(
    () => renderSegments(segments),
    [segments],
  );

  const handleWrapperClick = (): void => {
    if (textareaRef.current !== null) {
      textareaRef.current.focus();
    }
  };

  const handleTextareaScroll = (): void => {
    if (isSyncingScrollRef.current) {
      return;
    }

    if (textareaRef.current === null || overlayRef.current === null) {
      return;
    }

    isSyncingScrollRef.current = true;
    overlayRef.current.scrollTop = textareaRef.current.scrollTop;
    isSyncingScrollRef.current = false;
  };

  return (
    <div
      className="relative h-64 w-full rounded-md border border-slate-300 bg-white"
      onClick={handleWrapperClick}
    >
      <div
        className="pointer-events-none absolute inset-0 z-20 overflow-hidden whitespace-pre-wrap px-3 py-2 text-sm"
        aria-hidden="true"
        ref={overlayRef}
      >
        <div className="text-transparent">{highlightedContent}</div>
      </div>

      <textarea
        className="absolute inset-0 z-10 h-full w-full resize-none rounded-md border-none bg-transparent px-3 py-2 text-sm text-slate-900 outline-none focus-visible:ring-2 focus-visible:ring-sky-500"
        ref={textareaRef}
        value={value}
        onChange={(event) => onChange(event.target.value)}
        onScroll={handleTextareaScroll}
        placeholder={placeholder}
      />
    </div>
  );
}


