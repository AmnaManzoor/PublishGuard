export type ValidationIssueSeverity = 'Info' | 'Warning' | 'Error';

export type ValidationIssueCode =
  | 'ImagesTooFew'
  | 'ImagesTooMany'
  | 'ImageNotOnGoogleDrive'
  | 'ImageNotPublic'
  | 'ProductLinksTooFew'
  | 'ProductLinksTooMany'
  | 'MissingH2Headings'
  | 'ExcessiveBoldText';

export interface ValidationIssue {
  code: ValidationIssueCode;
  severity: ValidationIssueSeverity;
  message: string;
}
