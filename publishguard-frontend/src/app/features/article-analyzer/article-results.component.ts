import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { ArticleAnalysisResult } from '../../shared/models/article-analysis.model';
import { ValidationIssue, ValidationIssueCode } from '../../shared/models/validation-issue.model';

interface IssueGroups {
  imageIssues: ValidationIssue[];
  linkIssues: ValidationIssue[];
  formattingIssues: ValidationIssue[];
  otherIssues: ValidationIssue[];
}

@Component({
  selector: 'app-article-results',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './article-results.component.html',
  styleUrl: './article-results.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ArticleResultsComponent {
  private _result: ArticleAnalysisResult | null = null;

  @Input() set result(value: ArticleAnalysisResult | null) {
    this._result = value;
    this.issueGroups = buildIssueGroups(value?.issues ?? []);
  }
  get result(): ArticleAnalysisResult | null {
    return this._result;
  }

  @Input() uploadMessage: string | null = null;
  @Input() uploading: boolean | null = null;

  @Output() uploadRequested = new EventEmitter<void>();

  issueGroups: IssueGroups = {
    imageIssues: [],
    linkIssues: [],
    formattingIssues: [],
    otherIssues: []
  };

  get canUpload(): boolean {
    return this.result?.status === 'Ready';
  }

  get imageCount(): number {
    return this.result?.article.images.length ?? 0;
  }

  get productLinkCount(): number {
    return this.result?.article.links.filter((link) => link.isProductLink).length ?? 0;
  }

  get scoreClass(): string {
    return this.result ? `badge ${statusToClass(this.result.status)}` : 'badge';
  }

  requestUpload(): void {
    if (this.canUpload && !this.uploading) {
      this.uploadRequested.emit();
    }
  }
}

const imageIssueCodes: ValidationIssueCode[] = [
  'ImagesTooFew',
  'ImagesTooMany',
  'ImageNotOnGoogleDrive',
  'ImageNotPublic'
];

const linkIssueCodes: ValidationIssueCode[] = ['ProductLinksTooFew', 'ProductLinksTooMany'];

const formattingIssueCodes: ValidationIssueCode[] = ['MissingH2Headings', 'ExcessiveBoldText'];

function buildIssueGroups(issues: ValidationIssue[]): IssueGroups {
  const groups: IssueGroups = {
    imageIssues: [],
    linkIssues: [],
    formattingIssues: [],
    otherIssues: []
  };

  for (const issue of issues) {
    if (imageIssueCodes.includes(issue.code)) {
      groups.imageIssues.push(issue);
      continue;
    }

    if (linkIssueCodes.includes(issue.code)) {
      groups.linkIssues.push(issue);
      continue;
    }

    if (formattingIssueCodes.includes(issue.code)) {
      groups.formattingIssues.push(issue);
      continue;
    }

    groups.otherIssues.push(issue);
  }

  return groups;
}

function statusToClass(status: ArticleAnalysisResult['status']): string {
  switch (status) {
    case 'Ready':
      return 'badge--ready';
    case 'NeedsReview':
      return 'badge--review';
    default:
      return 'badge--blocked';
  }
}
