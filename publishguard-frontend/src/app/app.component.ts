import { Component } from '@angular/core';
import { ArticleAnalyzerComponent } from './features/article-analyzer/article-analyzer.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ArticleAnalyzerComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
}
