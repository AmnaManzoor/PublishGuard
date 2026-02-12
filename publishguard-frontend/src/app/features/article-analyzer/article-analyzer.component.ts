import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { BehaviorSubject, finalize } from 'rxjs';
import { ArticleService } from '../../core/services/article.service';
import { ArticleAnalysisResult } from '../../shared/models/article-analysis.model';
import { ArticleResultsComponent } from './article-results.component';

@Component({
  selector: 'app-article-analyzer',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ArticleResultsComponent],
  templateUrl: './article-analyzer.component.html',
  styleUrl: './article-analyzer.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ArticleAnalyzerComponent {
  private readonly resultSubject = new BehaviorSubject<ArticleAnalysisResult | null>(null);
  private readonly loadingSubject = new BehaviorSubject<boolean>(false);
  private readonly uploadMessageSubject = new BehaviorSubject<string | null>(null);
  private readonly errorSubject = new BehaviorSubject<string | null>(null);
  private readonly uploadingSubject = new BehaviorSubject<boolean>(false);

  readonly result$ = this.resultSubject.asObservable();
  readonly loading$ = this.loadingSubject.asObservable();
  readonly uploadMessage$ = this.uploadMessageSubject.asObservable();
  readonly error$ = this.errorSubject.asObservable();
  readonly uploading$ = this.uploadingSubject.asObservable();

  readonly form;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly articleService: ArticleService
  ) {
    this.form = this.formBuilder.group({
      sourceUrl: ['', [Validators.required]]
    });
  }

  analyze(): void {
    if (this.form.invalid) {
      this.errorSubject.next('Please provide a Google Docs URL.');
      return;
    }

    const sourceUrl = this.form.controls.sourceUrl.value ?? '';
    this.loadingSubject.next(true);
    this.errorSubject.next(null);
    this.uploadMessageSubject.next(null);

    this.articleService
      .analyze(sourceUrl)
      .pipe(finalize(() => this.loadingSubject.next(false)))
      .subscribe({
        next: (result) => this.resultSubject.next(result),
        error: (error: Error) => this.errorSubject.next(error.message)
      });
  }

  upload(): void {
    const result = this.resultSubject.value;
    if (!result) {
      return;
    }

    this.uploadingSubject.next(true);
    this.errorSubject.next(null);

    this.articleService
      .upload(result.article.sourceUrl)
      .pipe(finalize(() => this.uploadingSubject.next(false)))
      .subscribe({
        next: (response) => {
          this.resultSubject.next(response.result);
          this.uploadMessageSubject.next(response.message);
        },
        error: (error: Error) => this.errorSubject.next(error.message)
      });
  }
}
