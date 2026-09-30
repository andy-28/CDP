<script setup>
import { computed, ref } from 'vue'

const from = ref('2026-01-01')
const to = ref('2026-08-31')
const brands = ref('BRAND_A, BRAND_B')
const selectedMetrics = ref(['customer.uniqueCount', 'purchase.averageOrderValue'])
const result = ref(null)
const error = ref('')
const running = ref(false)

const maxBucketCount = computed(() => Math.max(1, ...(result.value?.distribution ?? []).map((row) => row.customerCount)))

async function runAnalysis() {
  error.value = ''
  result.value = null
  const brandValues = brands.value.split(',').map((brand) => brand.trim().toUpperCase()).filter(Boolean)
  if (!from.value || !to.value || !brandValues.length) {
    error.value = 'Complete the date range and provide at least one brand.'
    return
  }
  if (!selectedMetrics.value.length) {
    error.value = 'Select at least one metric.'
    return
  }

  running.value = true
  try {
    const response = await fetch('/api/analyses/run', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        timeRange: { from: from.value, to: to.value },
        timeGrain: 'month',
        filters: [{ field: 'purchase.brand', operator: 'in', value: brandValues }],
        dimensions: ['purchase.month'],
        metrics: selectedMetrics.value,
        buckets: [{
          field: 'purchase.customerPeriodAmount',
          ranges: [
            { label: 'Under 1,000', minimumInclusive: null, maximumExclusive: 1000 },
            { label: '1,000–2,999', minimumInclusive: 1000, maximumExclusive: 3000 },
            { label: '3,000–5,999', minimumInclusive: 3000, maximumExclusive: 6000 },
            { label: '6,000+', minimumInclusive: 6000, maximumExclusive: null },
          ],
        }],
      }),
    })
    const data = await response.json()
    if (!response.ok) throw new Error(data.error ?? 'Analysis failed.')
    result.value = data
  } catch (exception) {
    error.value = exception.message
  } finally {
    running.value = false
  }
}

function formatMoney(value) {
  return new Intl.NumberFormat('en-US', { maximumFractionDigits: 2 }).format(value)
}
</script>

<template>
<main class="analysis-workspace">
  <header class="topbar">
    <div>
      <div class="eyebrow">ANALYSIS</div>
      <h1>Brand spending analysis</h1>
    </div>
    <div class="environment"><span></span> DEMO / MOCK DATA</div>
  </header>

  <section class="analysis-builder builder-card">
    <div class="section-heading">
      <div>
        <h2>What happened?</h2>
        <p>Analyze validated metrics through semantic fields—without database details.</p>
      </div>
      <div class="use-case-tag">VALIDATED USE CASE 02</div>
    </div>

    <div class="analysis-form">
      <fieldset>
        <legend>Date range</legend>
        <div class="date-range">
          <label><span>From</span><input v-model="from" type="date" /></label>
          <i>to</i>
          <label><span>To</span><input v-model="to" type="date" /></label>
        </div>
      </fieldset>

      <fieldset>
        <legend>Filter</legend>
        <div class="analysis-filter-row">
          <label><span>Field</span><select disabled><option>Brand</option></select></label>
          <label><span>Operator</span><select disabled><option>in</option></select></label>
          <label class="brand-values"><span>Values</span><input v-model="brands" type="text" placeholder="BRAND_A, BRAND_B" /></label>
        </div>
        <small>Comma-separated mock brands only.</small>
      </fieldset>

      <div class="analysis-options">
        <fieldset>
          <legend>Group by</legend>
          <label><span>Time grain</span><select disabled><option>Month</option></select></label>
        </fieldset>
        <fieldset>
          <legend>Metrics</legend>
          <label class="check-option">
            <input v-model="selectedMetrics" type="checkbox" value="customer.uniqueCount" />
            <span><strong>Unique Customers</strong><small>Distinct customers per month</small></span>
          </label>
          <label class="check-option">
            <input v-model="selectedMetrics" type="checkbox" value="purchase.averageOrderValue" />
            <span><strong>Average Order Value</strong><small>Average mock purchase amount</small></span>
          </label>
        </fieldset>
      </div>
    </div>

    <div v-if="error" class="error-message" role="alert">{{ error }}</div>
    <div class="analysis-actions">
      <p>Results are calculated only from synthetic purchases.</p>
      <button class="preview-button run-analysis" type="button" :disabled="running" @click="runAnalysis">
        <span v-if="running" class="spinner"></span>{{ running ? 'Running…' : 'Run analysis' }}
      </button>
    </div>
  </section>

  <section v-if="result" class="analysis-results">
    <article class="results-card monthly-results">
      <div class="results-heading">
        <div><div class="eyebrow">MONTHLY RESULTS</div><h2>{{ result.rows.length }} periods</h2></div>
        <span class="mock-badge">{{ result.dataClassification }}</span>
      </div>
      <div class="table-wrap">
        <table>
          <thead><tr><th>Month</th><th v-if="selectedMetrics.includes('customer.uniqueCount')">Unique customers</th><th v-if="selectedMetrics.includes('purchase.averageOrderValue')">Avg order value</th></tr></thead>
          <tbody>
            <tr v-for="row in result.rows" :key="row.period">
              <td><strong>{{ row.period }}</strong></td>
              <td v-if="selectedMetrics.includes('customer.uniqueCount')">{{ row.uniqueCustomers }}</td>
              <td v-if="selectedMetrics.includes('purchase.averageOrderValue')">{{ formatMoney(row.averageOrderValue) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </article>

    <article class="results-card distribution-card">
      <div class="results-heading">
        <div><div class="eyebrow">SPENDING DISTRIBUTION</div><h2>Customers by period spend</h2></div>
      </div>
      <div class="bar-chart" aria-label="Customer spending distribution bar chart">
        <div v-for="row in result.distribution" :key="row.bucket" class="bar-row">
          <span class="bar-label">{{ row.bucket }}</span>
          <div class="bar-track"><div class="bar-fill" :style="{ width: `${(row.customerCount / maxBucketCount) * 100}%` }"></div></div>
          <strong>{{ row.customerCount }}</strong>
        </div>
      </div>
      <p class="chart-note">Buckets use each customer's total filtered spend across the selected period.</p>
    </article>
  </section>
</main>
</template>
