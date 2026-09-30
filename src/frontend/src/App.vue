<script setup>
import { computed, onMounted, ref } from 'vue'
import AnalysisWorkspace from './components/AnalysisWorkspace.vue'

const workspace = ref('audience')
const fields = ref([])
const operators = ref([])
const matchOperator = ref('and')
const loadingCatalog = ref(true)
const previewing = ref(false)
const error = ref('')
const result = ref(null)
const rules = ref([
  { id: crypto.randomUUID(), field: 'purchase.brand', operator: 'equals', value: 'NIKE', value2: '' },
  { id: crypto.randomUUID(), field: 'purchase.totalAmount', operator: 'greaterThanOrEqual', value: '5000', value2: '' },
])

const fieldMap = computed(() => new Map(fields.value.map((field) => [field.key, field])))
const operatorMap = computed(() => new Map(operators.value.map((operator) => [operator.key, operator])))

onMounted(async () => {
  try {
    const response = await fetch('/api/semantic-fields')
    if (!response.ok) throw new Error('Could not load the semantic catalog.')
    const data = await response.json()
    fields.value = data.fields.filter((field) => field.canFilter && field.supportedOperators.length)
    operators.value = data.operators
  } catch (exception) {
    error.value = `${exception.message} Is the backend running on port 5072?`
  } finally {
    loadingCatalog.value = false
  }
})

function availableOperators(rule) {
  const field = fieldMap.value.get(rule.field)
  return (field?.supportedOperators ?? []).map((key) => operatorMap.value.get(key)).filter(Boolean)
}

function fieldChanged(rule) {
  const allowed = fieldMap.value.get(rule.field)?.supportedOperators ?? []
  if (!allowed.includes(rule.operator)) rule.operator = allowed[0] ?? 'equals'
  rule.value = ''
  rule.value2 = ''
  result.value = null
}

function addRule() {
  const field = fields.value[0]
  rules.value.push({
    id: crypto.randomUUID(),
    field: field?.key ?? 'customer.memberLevel',
    operator: field?.supportedOperators?.[0] ?? 'equals',
    value: '',
    value2: '',
  })
}

function removeRule(id) {
  if (rules.value.length === 1) return
  rules.value = rules.value.filter((rule) => rule.id !== id)
  result.value = null
}

function inputType(rule) {
  const type = fieldMap.value.get(rule.field)?.dataType
  if (type === 'number' || type === 'numberOfDays') return 'number'
  if (type === 'date') return 'date'
  return 'text'
}

function serializeValue(rule) {
  const type = fieldMap.value.get(rule.field)?.dataType
  if (rule.operator === 'between') {
    return type === 'number' || type === 'numberOfDays'
      ? [Number(rule.value), Number(rule.value2)]
      : [rule.value, rule.value2]
  }
  if (rule.operator === 'in') return rule.value.split(',').map((value) => value.trim()).filter(Boolean)
  if (type === 'number' || type === 'numberOfDays') return Number(rule.value)
  return rule.value
}

async function previewAudience() {
  error.value = ''
  result.value = null
  if (rules.value.some((rule) => rule.value === '' || (rule.operator === 'between' && rule.value2 === ''))) {
    error.value = 'Complete every condition before previewing the audience.'
    return
  }

  previewing.value = true
  try {
    const response = await fetch('/api/audiences/preview', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        operator: matchOperator.value,
        rules: rules.value.map((rule) => ({
          field: rule.field,
          operator: rule.operator,
          value: serializeValue(rule),
        })),
        groups: [],
      }),
    })
    const data = await response.json()
    if (!response.ok) throw new Error(data.error ?? 'Audience preview failed.')
    result.value = data
  } catch (exception) {
    error.value = exception.message
  } finally {
    previewing.value = false
  }
}

function formatMoney(amount) {
  return new Intl.NumberFormat('en-US', { maximumFractionDigits: 0 }).format(amount)
}
</script>

<template>
  <div class="app-shell">
    <aside class="sidebar">
      <div class="brand-mark"><span>m</span></div>
      <nav aria-label="Primary navigation">
        <button class="nav-item" :class="{ active: workspace === 'audience' }" type="button" aria-label="Audience" title="Audience" @click="workspace = 'audience'">
          <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2M9 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8ZM22 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75"/></svg>
        </button>
        <button class="nav-item" :class="{ active: workspace === 'analysis' }" type="button" aria-label="Analysis" title="Analysis" @click="workspace = 'analysis'">
          <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 19V9M10 19V5M16 19v-7M22 19V2M2 19h22"/></svg>
        </button>
        <button class="nav-item" type="button" aria-label="Data sources" title="Data sources (future)" disabled>
          <svg viewBox="0 0 24 24" aria-hidden="true"><ellipse cx="12" cy="5" rx="8" ry="3"/><path d="M4 5v7c0 1.7 3.6 3 8 3s8-1.3 8-3V5M4 12v7c0 1.7 3.6 3 8 3s8-1.3 8-3v-7"/></svg>
        </button>
      </nav>
      <div class="demo-dot" title="Mock data source online"></div>
    </aside>

    <main v-if="workspace === 'audience'">
      <header class="topbar">
        <div>
          <div class="eyebrow">AUDIENCES</div>
          <h1>Create segment</h1>
        </div>
        <div class="environment"><span></span> DEMO / MOCK DATA</div>
      </header>

      <div class="content-grid">
        <section class="builder-card">
          <div class="section-heading">
            <div>
              <h2>Who should be included?</h2>
              <p>Build an audience using semantic customer and purchase fields.</p>
            </div>
            <div class="match-control">
              <label for="match">Match</label>
              <select id="match" v-model="matchOperator">
                <option value="and">ALL conditions</option>
                <option value="or">ANY condition</option>
              </select>
            </div>
          </div>

          <div v-if="loadingCatalog" class="loading-line">Loading semantic fields…</div>
          <div v-else class="rule-list">
            <div v-for="(rule, index) in rules" :key="rule.id" class="rule-row">
              <div class="connector"><span v-if="index">{{ matchOperator.toUpperCase() }}</span></div>
              <div class="rule-number">{{ index + 1 }}</div>
              <label>
                <span>Field</span>
                <select v-model="rule.field" @change="fieldChanged(rule)">
                  <option v-for="field in fields" :key="field.key" :value="field.key">{{ field.label }}</option>
                </select>
              </label>
              <label>
                <span>Operator</span>
                <select v-model="rule.operator">
                  <option v-for="operator in availableOperators(rule)" :key="operator.key" :value="operator.key">
                    {{ operator.label }}
                  </option>
                </select>
              </label>
              <label class="value-field" :class="{ between: rule.operator === 'between' }">
                <span>Value</span>
                <div class="value-inputs">
                  <input
                    v-model="rule.value"
                    :type="inputType(rule)"
                    :placeholder="rule.operator === 'in' ? 'NIKE, adidas' : rule.operator === 'between' ? 'From' : 'Enter a value'"
                    @input="result = null"
                  />
                  <template v-if="rule.operator === 'between'">
                    <em>to</em>
                    <input v-model="rule.value2" :type="inputType(rule)" placeholder="To" @input="result = null" />
                  </template>
                </div>
              </label>
              <button class="icon-button" type="button" :disabled="rules.length === 1" aria-label="Remove condition" @click="removeRule(rule.id)">×</button>
            </div>
          </div>

          <button class="add-button" type="button" :disabled="loadingCatalog" @click="addRule">
            <span>+</span> Add condition
          </button>

          <div v-if="error" class="error-message" role="alert">{{ error }}</div>
        </section>

        <aside class="estimate-card">
          <div class="estimate-label">ESTIMATED AUDIENCE</div>
          <div class="estimate-count">
            <strong>{{ result?.count ?? '—' }}</strong>
            <span>customers</span>
          </div>
          <p v-if="result">Matched against the current mock dataset.</p>
          <p v-else>Preview to calculate the number of matching customers.</p>
          <button class="preview-button" type="button" :disabled="previewing || loadingCatalog" @click="previewAudience">
            <span v-if="previewing" class="spinner"></span>
            {{ previewing ? 'Calculating…' : 'Preview audience' }}
          </button>
          <div class="privacy-note">No company or production data is connected.</div>
        </aside>
      </div>

      <section v-if="result" class="results-card">
        <div class="results-heading">
          <div>
            <div class="eyebrow">AUDIENCE PREVIEW</div>
            <h2>{{ result.count }} matching customers</h2>
          </div>
          <span class="mock-badge">{{ result.dataClassification }}</span>
        </div>
        <div class="table-wrap">
          <table>
            <thead><tr><th>Customer</th><th>Member level</th><th>Total purchase</th><th>Orders</th><th>Last purchase</th><th>Days ago</th></tr></thead>
            <tbody>
              <tr v-for="customer in result.customers" :key="customer.customerId">
                <td><strong>{{ customer.name }}</strong><small>{{ customer.customerId }}</small></td>
                <td><span class="level-pill">{{ customer.memberLevel }}</span></td>
                <td>{{ formatMoney(customer.totalPurchase) }}</td>
                <td>{{ customer.orderCount }}</td>
                <td>{{ customer.lastPurchaseDate ?? '—' }}</td>
                <td>{{ customer.daysSinceLastPurchase ?? '—' }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>
    </main>
    <AnalysisWorkspace v-else />
  </div>
</template>
