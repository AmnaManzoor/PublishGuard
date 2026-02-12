import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AnalysisStatus,
  ArticleAnalysisResult,
  ArticleSummary,
  ImageInfo,
  LinkInfo,
  WordPressPayload
} from '../../shared/models/article-analysis.model';
import {
  ValidationIssue,
  ValidationIssueCode,
  ValidationIssueSeverity
} from '../../shared/models/validation-issue.model';

interface AnalysisResultResponse {
  article: ArticleSummaryResponse;
  issues: ValidationIssueResponse[];
  score: number;
  status: number | AnalysisStatus;
  payload: WordPressPayloadResponse;
}

interface UploadResponse {
  result: AnalysisResultResponse;
  message: string;
}

interface ArticleSummaryResponse {
  sourceUrl: string;
  title: string;
  html: string;
  wordCount: number;
  h2Count: number;
  boldTextPercentage: number;
  images: ImageInfoResponse[];
  links: LinkInfoResponse[];
}

interface ImageInfoResponse {
  url: string;
  altText: string;
  isGoogleDriveHosted: boolean;
  isPubliclyAccessible: boolean;
}

interface LinkInfoResponse {
  url: string;
  text: string;
  isProductLink: boolean;
}

interface ValidationIssueResponse {
  code: number | ValidationIssueCode;
  severity: number | ValidationIssueSeverity;
  message: string;
}

interface WordPressPayloadResponse {
  title: string;
  contentHtml: string;
  excerpt: string;
}

const statusMap: AnalysisStatus[] = ['Ready', 'NeedsReview', 'Blocked'];
const severityMap: ValidationIssueSeverity[] = ['Info', 'Warning', 'Error'];
const codeMap: ValidationIssueCode[] = [
  'ImagesTooFew',
  'ImagesTooMany',
  'ImageNotOnGoogleDrive',
  'ImageNotPublic',
  'ProductLinksTooFew',
  'ProductLinksTooMany',
  'MissingH2Headings',
  'ExcessiveBoldText'
];

@Injectable({ providedIn: 'root' })
export class ArticleService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private readonly http: HttpClient) {}

  analyze(sourceUrl: string): Observable<ArticleAnalysisResult> {
    return this.http
      .post<AnalysisResultResponse>(`${this.baseUrl}/api/article/analyze`, { sourceUrl })
      .pipe(map(mapAnalysisResult), catchError((error) => this.handleError(error)));
  }

  upload(sourceUrl: string): Observable<{ result: ArticleAnalysisResult; message: string }> {
    return this.http
      .post<UploadResponse>(`${this.baseUrl}/api/article/upload`, { sourceUrl })
      .pipe(
        map((response) => ({
          result: mapAnalysisResult(response.result),
          message: response.message
        })),
        catchError((error) => this.handleError(error))
      );
  }

  private handleError(error: HttpErrorResponse) {
    const message =
      error.error?.message ||
      (typeof error.error === 'string' ? error.error : null) ||
      error.message ||
      'Something went wrong while contacting the server.';

    return throwError(() => new Error(message));
  }
}

function mapAnalysisResult(response: AnalysisResultResponse): ArticleAnalysisResult {
  return {
    article: mapArticle(response.article),
    issues: response.issues.map(mapIssue),
    score: response.score,
    status: normalizeStatus(response.status),
    payload: mapPayload(response.payload)
  };
}

function mapArticle(article: ArticleSummaryResponse): ArticleSummary {
  return {
    sourceUrl: article.sourceUrl,
    title: article.title,
    html: article.html,
    wordCount: article.wordCount,
    h2Count: article.h2Count,
    boldTextPercentage: article.boldTextPercentage,
    images: article.images.map(mapImage),
    links: article.links.map(mapLink)
  };
}

function mapImage(image: ImageInfoResponse): ImageInfo {
  return {
    url: image.url,
    altText: image.altText,
    isGoogleDriveHosted: image.isGoogleDriveHosted,
    isPubliclyAccessible: image.isPubliclyAccessible
  };
}

function mapLink(link: LinkInfoResponse): LinkInfo {
  return {
    url: link.url,
    text: link.text,
    isProductLink: link.isProductLink
  };
}

function mapIssue(issue: ValidationIssueResponse): ValidationIssue {
  return {
    code: normalizeCode(issue.code),
    severity: normalizeSeverity(issue.severity),
    message: issue.message
  };
}

function mapPayload(payload: WordPressPayloadResponse): WordPressPayload {
  return {
    title: payload.title,
    contentHtml: payload.contentHtml,
    excerpt: payload.excerpt
  };
}

function normalizeStatus(status: number | AnalysisStatus): AnalysisStatus {
  if (typeof status === 'number') {
    return statusMap[status] ?? 'NeedsReview';
  }

  return status;
}

function normalizeSeverity(severity: number | ValidationIssueSeverity): ValidationIssueSeverity {
  if (typeof severity === 'number') {
    return severityMap[severity] ?? 'Warning';
  }

  return severity;
}

function normalizeCode(code: number | ValidationIssueCode): ValidationIssueCode {
  if (typeof code === 'number') {
    return codeMap[code] ?? 'ExcessiveBoldText';
  }

  return code;
}
