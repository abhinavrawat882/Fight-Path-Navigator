import { Component,Renderer2,ElementRef,ViewChild } from '@angular/core';
import { HttpClient,HttpHeaders } from '@angular/common/http';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  //Rows:  latitude  --->  Column: Longitude
  weatherData: any[] =[[]];
  source ={i:-1,j:-1}
  destination ={i:-1,j:-1}
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    // Call your initialization logic here, such as fetching data from an API
    this.fetchData();
   
  }
  fetchData() {
    const headers = new HttpHeaders()
    .set('Content-Type', 'application/json');
    var dataSend={
      "endPointsLoc": {
        "source": {},
        "destination": {}
      },
      "currentLoc": {}
    }
    this.http.post<any>('https://localhost:7134/EndPoints/PathPrediction',dataSend,{headers:headers}).subscribe(data => {
      console.log('Received data:', data);
       this.weatherData=data.flightPath
       this.source.i=data.sourceI
       this.source.j =data.sourceJ
       this.destination.i=data.destinationI
       this.destination.j=data.destinationJ
       data.path.forEach((pathNode:any) => {
        const i = pathNode.i;
        const j = pathNode.j;
        if (this.weatherData[i] && this.weatherData[i][j]) {
          this.weatherData[i][j].isPath = true; // Add a property to mark as part of the path
        }

      });
      
    });
  }
  getWeatherIconClass(condition: string): string {
    switch (condition) {
      case 'Clear':
        return 'wb_sunny'; // Example: Use Font Awesome icon for clear weather
      case 'Partly Cloudy':
        return 'cloud_queue'; // Example: Use Font Awesome icon for partly cloudy weather
      case 'Cloudy':
        return 'cloud'; // Example: Use Font Awesome icon for cloudy weather
      case 'Rainy':
        return 'grain'; // Example: Use Font Awesome icon for rainy weather
      case 'Snowy':
        return 'cloud'; // Example: Use Font Awesome icon for snowy weather
      case 'ThunderStorm':
        return 'offline_bolt'; // Example: Use Font Awesome icon for snowy weather
      default:
        return 'offline_bolt'; // Default icon for unknown weather conditions
    }
  }
}
interface WeatherBlock {
  heuristicValue: number;
  weatherCondition: string;
}

interface GraphNode {
  i: number;
  j: number;
}

class AStar {
  constructor(private grid: any[][]) {}

  findShortestPath(start: GraphNode, end: GraphNode): GraphNode[] {
    console.log('Starting A* algorithm...');
    // Implement the A* algorithm here
    const open: GraphNode[] = [start];
    const closed: { [key: string]: boolean } = {}; // Dictionary for faster lookup

    while (open.length > 0) {
      const current = open.shift()!;
      console.log("Working on",current)
      if (current.i === end.i && current.j === end.j) {
        return this.reconstructPath(start, current);
      }

      const key = `${current.i},${current.j}`;
      closed[key] = true; // Mark current node as visited

      const neighbors = this.getNeighbors(current);
      for (const neighbor of neighbors) {
        const neighborKey = `${neighbor.i},${neighbor.j}`;
        if (closed[neighborKey]) {
          continue;
        }
        open.push(neighbor);
      }

      open.sort((a, b) => {
        const fA = this.grid[a.i][a.j].heuristicValue;
        const fB = this.grid[b.i][b.j].heuristicValue;
        return fA - fB;
      });
    }
    console.log('A* algorithm completed.');
    return [];
  }

  private getNeighbors(node: GraphNode): GraphNode[] {
    const neighbors: GraphNode[] = [];
    const { i, j } = node;

    // Assuming movements are allowed in eight directions (including diagonals)
    for (let di = -1; di <= 1; di++) {
      for (let dj = -1; dj <= 1; dj++) {
        if (di === 0 && dj === 0) continue; // Skip the current node
        const ni = i + di;
        const nj = j + dj;
        if (ni >= 0 && ni < this.grid.length && nj >= 0 && nj < this.grid[0].length) {
          neighbors.push({ i: ni, j: nj });
        }
      }
    }

    return neighbors;
  }

  private reconstructPath(start: GraphNode, current: GraphNode): GraphNode[] {
    const path: GraphNode[] = [current];
    while (current.i !== start.i || current.j !== start.j) {
      const neighbors = this.getNeighbors(current);
      for (const neighbor of neighbors) {
        if (this.grid[neighbor.i][neighbor.j].heuristicValue < this.grid[current.i][current.j].heuristicValue) {
          path.unshift(neighbor);
          current = neighbor;
          break;
        }
      }
    }
    return path;
  }
}

