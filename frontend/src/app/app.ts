import { Component, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  imports: [FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('Signal Ledger');
  metrics: any[] = [];
  premiumMethods: any[] = [];
  selectedMetric: any;
  result: any;
  imports: any[] = [];
  search = '';
  loading = false;
  message = '';
  showMetricForm = false;
  newMetric = { premiumMethodId: 1, code: '', name: '', description: '', sourceType: 'Excel', sourceName: '', ingestionFrequency: 'Monthly' };

  constructor(private http: HttpClient) {}

  ngOnInit(): void { this.loadMetrics(); this.loadPremiumMethods(); }

  loadPremiumMethods(): void {
    this.http.get<any[]>('http://localhost:5006/api/premium-methods').subscribe({ next: methods => {
      this.premiumMethods = methods;
      if (methods.length && !this.newMetric.premiumMethodId) this.newMetric.premiumMethodId = methods[0].id;
    } });
  }

  loadMetrics(): void {
    this.http.get<any[]>('http://localhost:5006/api/metrics').subscribe({ next: metrics => {
      this.metrics = metrics;
      if (!this.selectedMetric && metrics.length) this.selectMetric(metrics[0]);
    }, error: () => this.message = 'API unavailable. Start the backend and refresh.' });
  }

  selectMetric(metric: any): void { this.selectedMetric = metric; this.loadRows(); this.loadImports(); }
  createMetric(): void {
    this.http.post<any>('http://localhost:5006/api/metrics', this.newMetric).subscribe({ next: metric => {
      this.metrics = [...this.metrics, metric];
      this.showMetricForm = false;
      this.selectedMetric = metric;
      this.newMetric = { ...this.newMetric, code: '', name: '', description: '', sourceName: '' };
      this.loadRows();
      this.loadImports();
      this.message = `${metric.name} created. Select an Excel or CSV file to import its fields.`;
    }, error: error => this.message = error.error || 'Could not create metric.' });
  }
  loadRows(): void {
    if (!this.selectedMetric) return;
    this.loading = true;
    this.http.get<any>(`http://localhost:5006/api/metrics/${this.selectedMetric.id}/rows?page=1&pageSize=25&search=${encodeURIComponent(this.search)}`).subscribe({ next: result => { this.result = result; this.loading = false; }, error: () => { this.message = 'Could not load metric data.'; this.loading = false; } });
  }
  loadImports(): void { this.http.get<any[]>(`http://localhost:5006/api/metrics/${this.selectedMetric.id}/imports`).subscribe({ next: imports => this.imports = imports }); }
  upload(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file || !this.selectedMetric) return;
    const data = new FormData(); data.append('file', file); data.append('year', '2026'); data.append('period', 'Q2');
    this.http.post(`http://localhost:5006/api/metrics/${this.selectedMetric.id}/import`, data).subscribe({ next: () => { this.message = `${file.name} imported successfully`; this.loadRows(); this.loadImports(); }, error: () => this.message = 'Import failed. Check the file format and try again.' });
  }
}
