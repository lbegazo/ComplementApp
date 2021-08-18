import {
  AfterViewInit,
  Component,
  Inject,
  NgZone,
  OnDestroy,
  PLATFORM_ID,
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
// amCharts imports
import * as am4core from '@amcharts/amcharts4/core';
import * as am4charts from '@amcharts/amcharts4/charts';
import am4themes_animated from '@amcharts/amcharts4/themes/animated';

@Component({
  selector: 'app-graficoTest',
  templateUrl: './graficoTest.component.html',
  styleUrls: ['./graficoTest.component.scss'],
})
export class GraficoTestComponent implements OnDestroy, AfterViewInit {
  private chart: am4charts.XYChart;

  constructor(@Inject(PLATFORM_ID) private platformId, private zone: NgZone) {}

  // Run the function only in the browser
  browserOnly(f: () => void) {
    if (isPlatformBrowser(this.platformId)) {
      this.zone.runOutsideAngular(() => {
        f();
      });
    }
  }

  ngAfterViewInit() {
    /* Chart code */
    // Themes begin
    am4core.useTheme(am4themes_animated);
    // Themes end

    // Create chart instance
    const chart = am4core.create('chartdiv', am4charts.PieChart);

    // Add data
    chart.data = [
      {
        country: 'Ministerio de Minas y Energía',
        litres: 501.9,
      },
      {
        country: 'Ministerio de Hacienda y Crédito Público',
        litres: 301.9,
      },
      {
        country: 'Ministerio de Ambiente y Desarrollo Sostenible',
        litres: 201.1,
      },
      {
        country: 'Dirección de Impuestos y Aduanas Nacionales de Colombia - DIAN',
        litres: 165.8,
      },
      {
        country: 'Departamento Nacional de Estadística - DANE',
        litres: 139.9,
      },
      {
        country: 'Agencia Nacional de Hidrocarburos - ANH',
        litres: 128.3,
      },
      {
        country: 'Agencia Nacional de Minería - ANM',
        litres: 99,
      },
    ];

    // Add and configure Series
    const pieSeries = chart.series.push(new am4charts.PieSeries());
    pieSeries.dataFields.value = 'litres';
    pieSeries.dataFields.category = 'country';
    pieSeries.slices.template.stroke = am4core.color('#fff');
    pieSeries.slices.template.strokeOpacity = 1;

    // This creates initial animation
    pieSeries.hiddenState.properties.opacity = 1;
    pieSeries.hiddenState.properties.endAngle = -90;
    pieSeries.hiddenState.properties.startAngle = -90;

    chart.hiddenState.properties.radius = am4core.percent(0);
  }

  // ngAfterViewInit() {
  //   // Chart code goes in here
  //   this.browserOnly(() => {
  //     am4core.useTheme(am4themes_animated);

  //     const chart = am4core.create('chartdiv', am4charts.XYChart);

  //     chart.paddingRight = 20;

  //     const data = [];
  //     let visits = 10;
  //     for (let i = 1; i < 366; i++) {
  //       visits += Math.round(
  //         (Math.random() < 0.5 ? 1 : -1) * Math.random() * 10
  //       );
  //       data.push({
  //         date: new Date(2020, 0, i),
  //         name: 'name' + i,
  //         value: visits,
  //       });
  //     }

  //     chart.data = data;

  //     const dateAxis = chart.xAxes.push(new am4charts.DateAxis());
  //     dateAxis.renderer.grid.template.location = 0;

  //     const valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
  //     valueAxis.tooltip.disabled = true;
  //     valueAxis.renderer.minWidth = 35;

  //     const series = chart.series.push(new am4charts.LineSeries());
  //     series.dataFields.dateX = 'date';
  //     series.dataFields.valueY = 'value';
  //     series.tooltipText = '{valueY.value}';

  //     chart.cursor = new am4charts.XYCursor();

  //     const scrollbarX = new am4charts.XYChartScrollbar();
  //     scrollbarX.series.push(series);
  //     chart.scrollbarX = scrollbarX;

  //     this.chart = chart;
  //   });
  // }

  ngOnDestroy() {
    // Clean up chart when the component is removed
    this.browserOnly(() => {
      if (this.chart) {
        this.chart.dispose();
      }
    });
  }
}
