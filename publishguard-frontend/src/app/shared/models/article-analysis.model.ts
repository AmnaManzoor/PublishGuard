import { ValidationIssue } from './validation-issue.model';

export type AnalysisStatus = 'Ready' | 'NeedsReview' | 'Blocked';

export interface WordPressPayload {
  title: string;
  contentHtml: string;
  excerpt: string;
}

export interface ImageInfo {
  url: string;
  altText: string;
  isGoogleDriveHosted: boolean;
  isPubliclyAccessible: boolean;
}

export interface LinkInfo {
  url: string;
  text: string;
  isProductLink: boolean;
}

export interface ArticleSummary {
  sourceUrl: string;
  title: string;
  html: string;
  wordCount: number;
  h2Count: number;
  boldTextPercentage: number;
  images: ImageInfo[];
  links: LinkInfo[];
}

export interface ArticleAnalysisResult {
  article: ArticleSummary;
  issues: ValidationIssue[];
  score: number;
  status: AnalysisStatus;
  payload: WordPressPayload;
}
